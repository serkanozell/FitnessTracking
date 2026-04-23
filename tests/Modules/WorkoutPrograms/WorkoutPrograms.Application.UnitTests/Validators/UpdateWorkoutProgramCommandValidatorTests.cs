using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class UpdateWorkoutProgramCommandValidatorTests
{
    private readonly UpdateWorkoutProgramCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new UpdateWorkoutProgramCommand(Guid.NewGuid(), "PPL", null, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var command = new UpdateWorkoutProgramCommand(Guid.Empty, "PPL", null, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldFail_WhenNameIsEmpty()
    {
        var command = new UpdateWorkoutProgramCommand(Guid.NewGuid(), "", null, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenNameExceedsMaxLength()
    {
        var command = new UpdateWorkoutProgramCommand(Guid.NewGuid(), new string('A', 101), null, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenEndDateBeforeStartDate()
    {
        var command = new UpdateWorkoutProgramCommand(Guid.NewGuid(), "PPL", null, new DateTime(2025, 3, 31), new DateTime(2025, 1, 1));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
