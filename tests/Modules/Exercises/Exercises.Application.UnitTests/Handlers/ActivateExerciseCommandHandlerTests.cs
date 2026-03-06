using Exercises.Application.Features.Exercises.ActivateExercise;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Exercises.Application.UnitTests.Handlers;

public class ActivateExerciseCommandHandlerTests
{
    private readonly IExerciseRepository _exerciseRepository = Substitute.For<IExerciseRepository>();
    private readonly IExercisesUnitOfWork _unitOfWork = Substitute.For<IExercisesUnitOfWork>();
    private readonly ActivateExerciseCommandHandler _sut;

    public ActivateExerciseCommandHandlerTests()
    {
        _sut = new ActivateExerciseCommandHandler(_exerciseRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldActivateExercise_WhenExistsAndNotActive()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, null, "Desc");
        var command = new ActivateExerciseCommand(exercise.Id);
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(exercise.Id);
        exercise.IsActive.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var command = new ActivateExerciseCommand(Guid.NewGuid());
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Exercise.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyActiveError_WhenAlreadyActive()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, null, "Desc");
        exercise.Activate();
        var command = new ActivateExerciseCommand(exercise.Id);
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Exercise.AlreadyActive");
    }
}
