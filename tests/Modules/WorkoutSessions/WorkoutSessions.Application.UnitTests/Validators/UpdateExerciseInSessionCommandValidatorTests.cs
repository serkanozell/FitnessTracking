using FluentValidation.TestHelper;
using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.UpdateExerciseInSession;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class UpdateExerciseInSessionCommandValidatorTests
{
    private readonly UpdateExerciseInSessionCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new UpdateExerciseInSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, 80m, 10));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutSessionIdIsEmpty()
    {
        var result = _sut.TestValidate(new UpdateExerciseInSessionCommand(Guid.Empty, Guid.NewGuid(), 1, 80m, 10));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionId);
    }

    [Fact]
    public void ShouldFail_WhenSessionExerciseIdIsEmpty()
    {
        var result = _sut.TestValidate(new UpdateExerciseInSessionCommand(Guid.NewGuid(), Guid.Empty, 1, 80m, 10));
        result.ShouldHaveValidationErrorFor(x => x.SessionExerciseId);
    }

    [Fact]
    public void ShouldFail_WhenSetNumberIsZero()
    {
        var result = _sut.TestValidate(new UpdateExerciseInSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 0, 80m, 10));
        result.ShouldHaveValidationErrorFor(x => x.SetNumber);
    }

    [Fact]
    public void ShouldFail_WhenWeightIsNegative()
    {
        var result = _sut.TestValidate(new UpdateExerciseInSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, -1m, 10));
        result.ShouldHaveValidationErrorFor(x => x.Weight);
    }

    [Fact]
    public void ShouldFail_WhenRepsIsZero()
    {
        var result = _sut.TestValidate(new UpdateExerciseInSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, 80m, 0));
        result.ShouldHaveValidationErrorFor(x => x.Reps);
    }
}
