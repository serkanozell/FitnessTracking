using FluentValidation.TestHelper;
using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.ActivateSessionExercise;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class ActivateSessionExerciseCommandValidatorTests
{
    private readonly ActivateSessionExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new ActivateSessionExerciseCommand(Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutSessionIdIsEmpty()
    {
        var result = _sut.TestValidate(new ActivateSessionExerciseCommand(Guid.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionId);
    }

    [Fact]
    public void ShouldFail_WhenSessionExerciseIdIsEmpty()
    {
        var result = _sut.TestValidate(new ActivateSessionExerciseCommand(Guid.NewGuid(), Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.SessionExerciseId);
    }
}
