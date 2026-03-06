using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class ActivateSplitExerciseCommandValidatorTests
{
    private readonly ActivateSplitExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new ActivateSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutProgramIdIsEmpty()
    {
        var result = _sut.TestValidate(new ActivateSplitExerciseCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutProgramId);
    }

    [Fact]
    public void ShouldFail_WhenSplitIdIsEmpty()
    {
        var result = _sut.TestValidate(new ActivateSplitExerciseCommand(Guid.NewGuid(), Guid.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.SplitId);
    }

    [Fact]
    public void ShouldFail_WhenExerciseIdIsEmpty()
    {
        var result = _sut.TestValidate(new ActivateSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutSplitExerciseId);
    }
}
