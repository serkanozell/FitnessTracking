using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class AddExerciseToSplitCommandValidatorTests
{
    private readonly AddExerciseToSplitCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new AddExerciseToSplitCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, 8, 12));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenSetsIsZero()
    {
        var result = _sut.TestValidate(new AddExerciseToSplitCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, 8, 12));
        result.ShouldHaveValidationErrorFor(x => x.Sets);
    }

    [Fact]
    public void ShouldFail_WhenMinimumRepsIsZero()
    {
        var result = _sut.TestValidate(new AddExerciseToSplitCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, 0, 12));
        result.ShouldHaveValidationErrorFor(x => x.MinimumReps);
    }

    [Fact]
    public void ShouldFail_WhenMaximumRepsLessThanMinimumReps()
    {
        var result = _sut.TestValidate(new AddExerciseToSplitCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, 12, 8));
        result.ShouldHaveValidationErrorFor(x => x.MaximumReps);
    }

    [Fact]
    public void ShouldFail_WhenExerciseIdIsEmpty()
    {
        var result = _sut.TestValidate(new AddExerciseToSplitCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, 4, 8, 12));
        result.ShouldHaveValidationErrorFor(x => x.ExerciseId);
    }
}
