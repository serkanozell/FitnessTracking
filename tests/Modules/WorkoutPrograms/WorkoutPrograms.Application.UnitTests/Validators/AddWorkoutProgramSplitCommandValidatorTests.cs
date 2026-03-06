using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class AddWorkoutProgramSplitCommandValidatorTests
{
    private readonly AddWorkoutProgramSplitCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new AddWorkoutProgramSplitCommand(Guid.NewGuid(), "Push Day", 1);
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutProgramIdIsEmpty()
    {
        var command = new AddWorkoutProgramSplitCommand(Guid.Empty, "Push Day", 1);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WorkoutProgramId);
    }

    [Fact]
    public void ShouldFail_WhenNameIsEmpty()
    {
        var command = new AddWorkoutProgramSplitCommand(Guid.NewGuid(), "", 1);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenOrderIsZero()
    {
        var command = new AddWorkoutProgramSplitCommand(Guid.NewGuid(), "Push Day", 0);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Order);
    }
}
