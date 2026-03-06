using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class UpdateSplitExerciseCommandValidatorTests
{
    private readonly UpdateSplitExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new UpdateSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, 8, 12));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenSetsIsZero()
    {
        var result = _sut.TestValidate(new UpdateSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, 8, 12));
        result.ShouldHaveValidationErrorFor(x => x.Sets);
    }

    [Fact]
    public void ShouldFail_WhenMaxRepsLessThanMinReps()
    {
        var result = _sut.TestValidate(new UpdateSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, 12, 8));
        result.ShouldHaveValidationErrorFor(x => x.MaximumReps);
    }
}
