using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class RemoveSplitExerciseCommandValidatorTests
{
    private readonly RemoveSplitExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new RemoveSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenAnyIdIsEmpty()
    {
        var result = _sut.TestValidate(new RemoveSplitExerciseCommand(Guid.Empty, Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutProgramId);
    }
}
