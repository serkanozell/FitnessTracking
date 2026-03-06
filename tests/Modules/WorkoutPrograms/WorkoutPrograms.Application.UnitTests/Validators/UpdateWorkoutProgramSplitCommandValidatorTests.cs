using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class UpdateWorkoutProgramSplitCommandValidatorTests
{
    private readonly UpdateWorkoutProgramSplitCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new UpdateWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.NewGuid(), "Push Day", 1));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenNameIsEmpty()
    {
        var result = _sut.TestValidate(new UpdateWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.NewGuid(), "", 1));
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenOrderIsZero()
    {
        var result = _sut.TestValidate(new UpdateWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.NewGuid(), "Push Day", 0));
        result.ShouldHaveValidationErrorFor(x => x.Order);
    }
}
