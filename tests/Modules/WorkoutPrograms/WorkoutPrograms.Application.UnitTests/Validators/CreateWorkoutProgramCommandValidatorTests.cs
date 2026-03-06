using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class CreateWorkoutProgramCommandValidatorTests
{
    private readonly CreateWorkoutProgramCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new CreateWorkoutProgramCommand("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldFail_WhenNameIsEmpty(string? name)
    {
        var command = new CreateWorkoutProgramCommand(name!, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenNameExceedsMaxLength()
    {
        var command = new CreateWorkoutProgramCommand(new string('A', 101), new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenEndDateBeforeStartDate()
    {
        var command = new CreateWorkoutProgramCommand("PPL", new DateTime(2025, 3, 31), new DateTime(2025, 1, 1));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
