using FluentValidation.TestHelper;
using WorkoutSessions.Application.Feature.WorkoutSessions.CreateWorkoutSession;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Validators;

public class CreateWorkoutSessionCommandValidatorTests
{
    private readonly CreateWorkoutSessionCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new CreateWorkoutSessionCommand(Guid.NewGuid(), new DateTime(2025, 6, 15));
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenWorkoutProgramIdIsEmpty()
    {
        var command = new CreateWorkoutSessionCommand(Guid.Empty, new DateTime(2025, 6, 15));
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WorkoutProgramId);
    }

    [Fact]
    public void ShouldFail_WhenDateIsDefault()
    {
        var command = new CreateWorkoutSessionCommand(Guid.NewGuid(), default);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }
}
