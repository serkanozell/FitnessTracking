using FluentAssertions;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class CreateWorkoutProgramCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly CreateWorkoutProgramCommandHandler _sut;

    public CreateWorkoutProgramCommandHandlerTests()
    {
        _sut = new CreateWorkoutProgramCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateProgram_WhenDateRangeIsValid()
    {
        var command = new CreateWorkoutProgramCommand("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Any<WorkoutProgram>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidDateRangeError_WhenEndDateBeforeStartDate()
    {
        var command = new CreateWorkoutProgramCommand("PPL", new DateTime(2025, 3, 31), new DateTime(2025, 1, 1));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.InvalidDateRange");
        await _repository.DidNotReceive().AddAsync(Arg.Any<WorkoutProgram>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidDateRangeError_WhenEndDateEqualsStartDate()
    {
        var sameDate = new DateTime(2025, 1, 1);
        var command = new CreateWorkoutProgramCommand("PPL", sameDate, sameDate);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.InvalidDateRange");
    }
}
