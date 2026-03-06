using FluentValidation.TestHelper;
using WorkoutSessions.Application.Feature.WorkoutSessions.UpdateWorkoutSession;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class UpdateWorkoutSessionCommandValidatorTests
{
    private readonly UpdateWorkoutSessionCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var result = _sut.TestValidate(new UpdateWorkoutSessionCommand(Guid.NewGuid(), new DateTime(2025, 7, 1)));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var result = _sut.TestValidate(new UpdateWorkoutSessionCommand(Guid.Empty, new DateTime(2025, 7, 1)));
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldFail_WhenDateIsDefault()
    {
        var result = _sut.TestValidate(new UpdateWorkoutSessionCommand(Guid.NewGuid(), default));
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }
}
