using Exercises.Application.Features.Exercises.CreateExercise;
using Exercises.Application.Features.Exercises.UpdateExercise;
using Exercises.Application.Features.Exercises.DeleteExercise;
using Exercises.Application.Features.Exercises.ActivateExercise;
using FluentValidation.TestHelper;
using Xunit;

namespace Exercises.Application.UnitTests.Validators;

public class CreateExerciseCommandValidatorTests
{
    private readonly CreateExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new CreateExerciseCommand("Bench Press", "Chest", "Triceps", "Flat bench", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldFail_WhenNameIsEmpty(string? name)
    {
        var command = new CreateExerciseCommand(name!, "Chest", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenNameExceedsMaxLength()
    {
        var command = new CreateExerciseCommand(new string('A', 101), "Chest", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenPrimaryMuscleGroupIsInvalid()
    {
        var command = new CreateExerciseCommand("Bench", "InvalidGroup", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PrimaryMuscleGroup);
    }

    [Fact]
    public void ShouldFail_WhenSecondaryMuscleGroupIsInvalid()
    {
        var command = new CreateExerciseCommand("Bench", "Chest", "InvalidGroup", "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SecondaryMuscleGroup);
    }

    [Fact]
    public void ShouldPass_WhenSecondaryMuscleGroupIsNull()
    {
        var command = new CreateExerciseCommand("Bench", "Chest", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.SecondaryMuscleGroup);
    }

    [Fact]
    public void ShouldFail_WhenDescriptionExceedsMaxLength()
    {
        var command = new CreateExerciseCommand("Bench", "Chest", null, new string('A', 1001), null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}

public class UpdateExerciseCommandValidatorTests
{
    private readonly UpdateExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new UpdateExerciseCommand(Guid.NewGuid(), "Bench", "Chest", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var command = new UpdateExerciseCommand(Guid.Empty, "Bench", "Chest", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldFail_WhenNameIsEmpty()
    {
        var command = new UpdateExerciseCommand(Guid.NewGuid(), "", "Chest", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void ShouldFail_WhenPrimaryMuscleGroupIsInvalid()
    {
        var command = new UpdateExerciseCommand(Guid.NewGuid(), "Bench", "Invalid", null, "Desc", null, null);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PrimaryMuscleGroup);
    }
}

public class DeleteExerciseCommandValidatorTests
{
    private readonly DeleteExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenIdIsValid()
    {
        var command = new DeleteExerciseCommand(Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var command = new DeleteExerciseCommand(Guid.Empty);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

public class ActivateExerciseCommandValidatorTests
{
    private readonly ActivateExerciseCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenIdIsValid()
    {
        var command = new ActivateExerciseCommand(Guid.NewGuid());
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenIdIsEmpty()
    {
        var command = new ActivateExerciseCommand(Guid.Empty);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
