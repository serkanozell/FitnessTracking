using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class ActivateWorkoutProgramCommandValidatorTests
{
    private readonly ActivateWorkoutProgramCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenIdIsValid()
    {
        var result = _sut.TestValidate(new ActivateWorkoutProgramCommand(Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var result = _sut.TestValidate(new ActivateWorkoutProgramCommand(Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
