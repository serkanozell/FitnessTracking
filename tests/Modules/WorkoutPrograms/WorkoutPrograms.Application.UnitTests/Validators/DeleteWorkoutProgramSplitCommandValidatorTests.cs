using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class DeleteWorkoutProgramSplitCommandValidatorTests
{
    private readonly DeleteWorkoutProgramSplitCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new DeleteWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutProgramIdIsEmpty()
    {
        var result = _sut.TestValidate(new DeleteWorkoutProgramSplitCommand(Guid.Empty, Guid.NewGuid()));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutProgramId);
    }

    [Fact]
    public void ShouldFail_WhenSplitIdIsEmpty()
    {
        var result = _sut.TestValidate(new DeleteWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.SplitId);
    }
}
