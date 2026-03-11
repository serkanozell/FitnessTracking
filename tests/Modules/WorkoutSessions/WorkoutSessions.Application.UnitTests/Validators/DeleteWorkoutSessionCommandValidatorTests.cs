using FluentValidation.TestHelper;
using WorkoutSessions.Application.Features.WorkoutSessions.DeleteWorkoutSession;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class DeleteWorkoutSessionCommandValidatorTests
{
    private readonly DeleteWorkoutSessionCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenIdIsValid()
    {
        var command = new DeleteWorkoutSessionCommand(Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var command = new DeleteWorkoutSessionCommand(Guid.Empty);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
