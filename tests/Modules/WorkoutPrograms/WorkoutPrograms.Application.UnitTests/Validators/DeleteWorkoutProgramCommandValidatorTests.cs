using FluentValidation.TestHelper;
using WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Validators;

public class DeleteWorkoutProgramCommandValidatorTests
{
    private readonly DeleteWorkoutProgramCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenIdIsValid()
    {
        var command = new DeleteWorkoutProgramCommand(Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var command = new DeleteWorkoutProgramCommand(Guid.Empty);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
