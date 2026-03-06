using FluentValidation.TestHelper;
using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class RemoveExerciseFromSessionCommandValidatorTests
{
    private readonly RemoveExerciseFromSessionCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new RemoveExerciseFromSessionCommand(Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutSessionIdIsEmpty()
    {
        var result = _sut.TestValidate(new RemoveExerciseFromSessionCommand(Guid.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionId);
    }

    [Fact]
    public void ShouldFail_WhenSessionExerciseIdIsEmpty()
    {
        var result = _sut.TestValidate(new RemoveExerciseFromSessionCommand(Guid.NewGuid(), Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.SessionExerciseId);
    }
}
