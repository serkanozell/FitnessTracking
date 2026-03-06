using FluentValidation.TestHelper;
using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class AddExerciseToSessionCommandValidatorTests
{
    private readonly AddExerciseToSessionCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new AddExerciseToSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, 80m, 10);
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutSessionIdIsEmpty()
    {
        var command = new AddExerciseToSessionCommand(Guid.Empty, Guid.NewGuid(), 1, 80m, 10);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionId);
    }

    [Fact]
    public void ShouldFail_WhenExerciseIdIsEmpty()
    {
        var command = new AddExerciseToSessionCommand(Guid.NewGuid(), Guid.Empty, 1, 80m, 10);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ExerciseId);
    }

    [Fact]
    public void ShouldFail_WhenSetNumberIsZero()
    {
        var command = new AddExerciseToSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 0, 80m, 10);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SetNumber);
    }

    [Fact]
    public void ShouldFail_WhenWeightIsNegative()
    {
        var command = new AddExerciseToSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, -1m, 10);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }

    [Fact]
    public void ShouldFail_WhenRepsIsZero()
    {
        var command = new AddExerciseToSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, 80m, 0);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Reps);
    }
}
