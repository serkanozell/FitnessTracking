using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class UpdateWorkoutProgramCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly UpdateWorkoutProgramCommandHandler _sut;

    public UpdateWorkoutProgramCommandHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _sut = new UpdateWorkoutProgramCommandHandler(_repository, _unitOfWork, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProgram_WhenExistsAndDateRangeValid()
    {
        var program = WorkoutProgram.Create(TestUserId, "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new UpdateWorkoutProgramCommand(program.Id, "Updated", new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        program.Name.Should().Be("Updated");
        program.StartDate.Should().Be(new DateTime(2025, 4, 1));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var command = new UpdateWorkoutProgramCommand(Guid.NewGuid(), "Name", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidDateRangeError_WhenEndDateBeforeStartDate()
    {
        var program = WorkoutProgram.Create(TestUserId, "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new UpdateWorkoutProgramCommand(program.Id, "Name", new DateTime(2025, 6, 1), new DateTime(2025, 1, 1));
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.InvalidDateRange");
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenUserDoesNotOwnProgram()
    {
        var otherUserId = Guid.NewGuid();
        var program = WorkoutProgram.Create(otherUserId, "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new UpdateWorkoutProgramCommand(program.Id, "Updated", new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Error.Forbidden");
    }

    [Fact]
    public async Task Handle_ShouldAllowAdmin_WhenUserDoesNotOwnProgram()
    {
        var otherUserId = Guid.NewGuid();
        var program = WorkoutProgram.Create(otherUserId, "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new UpdateWorkoutProgramCommand(program.Id, "Updated", new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(program);
        _currentUser.IsAdmin.Returns(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
