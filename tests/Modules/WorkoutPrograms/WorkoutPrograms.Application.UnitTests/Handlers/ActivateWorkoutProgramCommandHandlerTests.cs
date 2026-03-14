using FluentAssertions;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class ActivateWorkoutProgramCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly ActivateWorkoutProgramCommandHandler _sut;

    public ActivateWorkoutProgramCommandHandlerTests()
    {
        _sut = new ActivateWorkoutProgramCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldActivateProgram_WhenExistsAndNotActive()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new ActivateWorkoutProgramCommand(program.Id);
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(program.Id);
        program.IsActive.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var command = new ActivateWorkoutProgramCommand(Guid.NewGuid());
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyActiveError_WhenAlreadyActive()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.Activate();
        var command = new ActivateWorkoutProgramCommand(program.Id);
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.AlreadyActive");
    }
}
