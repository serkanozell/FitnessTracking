using FluentValidation.TestHelper;
using WorkoutSessions.Application.Features.WorkoutSessions.ActivateWorkoutSession;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class ActivateWorkoutSessionCommandValidatorTests
{
    private readonly ActivateWorkoutSessionCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenIdIsValid()
    {
        var result = _sut.TestValidate(new ActivateWorkoutSessionCommand(Guid.NewGuid()));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var result = _sut.TestValidate(new ActivateWorkoutSessionCommand(Guid.Empty));
        result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionId);
    }
}
