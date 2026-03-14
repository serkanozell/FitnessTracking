using BuildingBlocks.Domain.Exceptions;
using FluentAssertions;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.ValueObjects;
using WorkoutPrograms.Domain.Events;
using Xunit;

namespace WorkoutPrograms.Domain.UnitTests;

public class WorkoutProgramTests
{
    private static readonly DateTime DefaultStart = new(2025, 1, 1);
    private static readonly DateTime DefaultEnd = new(2025, 3, 31);

    private static WorkoutProgram CreateDefaultProgram()
        => WorkoutProgram.Create(Guid.NewGuid(), "PPL Program", DefaultStart, DefaultEnd);

    private static WorkoutProgram CreateActiveProgramWithSplit(out Guid splitId)
    {
        var program = CreateDefaultProgram();
        program.Activate();
        var split = program.AddSplit("Push Day", 1);
        splitId = split.Id;
        program.ClearDomainEvents();
        return program;
    }

    #region Create

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "PPL Program", DefaultStart, DefaultEnd);

        program.Id.Should().NotBeEmpty();
        program.Name.Should().Be("PPL Program");
        program.StartDate.Should().Be(DefaultStart);
        program.EndDate.Should().Be(DefaultEnd);
        program.Splits.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldRaiseWorkoutProgramCreatedEvent()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "PPL Program", DefaultStart, DefaultEnd);

        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramCreatedEvent>()
            .Which.ProgramId.Should().Be(program.Id);
    }

    #endregion

    #region Update

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var program = CreateDefaultProgram();
        var newStart = new DateTime(2025, 4, 1);
        var newEnd = new DateTime(2025, 6, 30);

        program.Update("Updated Program", newStart, newEnd);

        program.Name.Should().Be("Updated Program");
        program.StartDate.Should().Be(newStart);
        program.EndDate.Should().Be(newEnd);
    }

    [Fact]
    public void Update_ShouldRaiseWorkoutProgramUpdatedEvent()
    {
        var program = CreateDefaultProgram();
        program.ClearDomainEvents();

        program.Update("Updated", DefaultStart, DefaultEnd);

        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramUpdatedEvent>()
            .Which.ProgramId.Should().Be(program.Id);
    }

    #endregion

    #region Activate / Delete

    [Fact]
    public void Activate_ShouldSetIsActiveTrueAndIsDeletedFalse()
    {
        var program = CreateDefaultProgram();
        program.Delete();
        program.ClearDomainEvents();

        program.Activate();

        program.IsActive.Should().BeTrue();
        program.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldRaiseWorkoutProgramActivatedEvent()
    {
        var program = CreateDefaultProgram();
        program.ClearDomainEvents();

        program.Activate();

        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramActivatedEvent>();
    }

    [Fact]
    public void Delete_ShouldSetIsActiveFalseAndIsDeletedTrue()
    {
        var program = CreateDefaultProgram();
        program.Activate();
        program.ClearDomainEvents();

        program.Delete();

        program.IsActive.Should().BeFalse();
        program.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_ShouldRaiseWorkoutProgramDeletedEvent()
    {
        var program = CreateDefaultProgram();
        program.ClearDomainEvents();

        program.Delete();

        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramDeletedEvent>();
    }

    #endregion

    #region AddSplit

    [Fact]
    public void AddSplit_ShouldAddSplitToCollection()
    {
        var program = CreateDefaultProgram();
        program.ClearDomainEvents();

        var split = program.AddSplit("Push Day", 1);

        program.Splits.Should().ContainSingle()
            .Which.Id.Should().Be(split.Id);
        split.Name.Should().Be("Push Day");
        split.Order.Should().Be(1);
        split.WorkoutProgramId.Should().Be(program.Id);
    }

    [Fact]
    public void AddSplit_ShouldRaiseWorkoutProgramSplitChangedEvent()
    {
        var program = CreateDefaultProgram();
        program.ClearDomainEvents();

        program.AddSplit("Push Day", 1);

        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramSplitChangedEvent>()
            .Which.ProgramId.Should().Be(program.Id);
    }

    [Fact]
    public void AddSplit_WithDuplicateName_ShouldThrowBusinessRuleViolationException()
    {
        var program = CreateDefaultProgram();
        program.AddSplit("Push Day", 1);

        var act = () => program.AddSplit("Push Day", 2);

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Push Day*already exists*");
    }

    #endregion

    #region UpdateSplit

    [Fact]
    public void UpdateSplit_ShouldChangeProperties()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);

        program.UpdateSplit(splitId, "Pull Day", 2);

        var split = program.Splits.Single(s => s.Id == splitId);
        split.Name.Should().Be("Pull Day");
        split.Order.Should().Be(2);
    }

    [Fact]
    public void UpdateSplit_ShouldRaiseWorkoutProgramSplitChangedEvent()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);

        program.UpdateSplit(splitId, "Pull Day", 2);

        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramSplitChangedEvent>();
    }

    [Fact]
    public void UpdateSplit_WithInvalidSplitId_ShouldThrowDomainNotFoundException()
    {
        var program = CreateDefaultProgram();
        var invalidId = Guid.NewGuid();

        var act = () => program.UpdateSplit(invalidId, "Pull Day", 2);

        act.Should().Throw<DomainNotFoundException>();
    }

    #endregion

    #region ActivateSplit

    [Fact]
    public void ActivateSplit_ShouldActivateSplitAndRaiseEvent()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);

        program.ActivateSplit(splitId);

        var split = program.Splits.Single(s => s.Id == splitId);
        split.IsActive.Should().BeTrue();
        split.IsDeleted.Should().BeFalse();
        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramSplitChangedEvent>();
    }

    [Fact]
    public void ActivateSplit_WhenProgramIsInactive_ShouldThrowBusinessRuleViolationException()
    {
        var program = CreateDefaultProgram();
        program.AddSplit("Push Day", 1);
        var splitId = program.Splits.First().Id;

        var act = () => program.ActivateSplit(splitId);

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*not active*");
    }

    [Fact]
    public void ActivateSplit_WhenSplitNotFound_ShouldThrowDomainNotFoundException()
    {
        var program = CreateDefaultProgram();
        program.Activate();
        program.ClearDomainEvents();

        var act = () => program.ActivateSplit(Guid.NewGuid());

        act.Should().Throw<DomainNotFoundException>();
    }

    #endregion

    #region RemoveSplit

    [Fact]
    public void RemoveSplit_ShouldRemoveSplitFromCollection()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);

        program.RemoveSplit(splitId);

        program.Splits.Should().BeEmpty();
    }

    [Fact]
    public void RemoveSplit_ShouldRaiseWorkoutProgramSplitChangedEvent()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);

        program.RemoveSplit(splitId);

        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutProgramSplitChangedEvent>();
    }

    [Fact]
    public void RemoveSplit_WhenSplitNotFound_ShouldNotThrow()
    {
        var program = CreateDefaultProgram();
        program.ClearDomainEvents();

        var act = () => program.RemoveSplit(Guid.NewGuid());

        act.Should().NotThrow();
        program.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region AddExerciseToSplit

    [Fact]
    public void AddExerciseToSplit_ShouldAddExerciseAndRaiseEvent()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);
        var exerciseId = Guid.NewGuid();

        var exercise = program.AddExerciseToSplit(splitId, exerciseId, 4, new RepRange(8, 12));

        exercise.ExerciseId.Should().Be(exerciseId);
        exercise.Sets.Should().Be(4);
        exercise.RepRange.Minimum.Should().Be(8);
        exercise.RepRange.Maximum.Should().Be(12);
        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SplitExerciseChangedEvent>();
    }

    [Fact]
    public void AddExerciseToSplit_WithInvalidSplitId_ShouldThrowDomainNotFoundException()
    {
        var program = CreateDefaultProgram();

        var act = () => program.AddExerciseToSplit(Guid.NewGuid(), Guid.NewGuid(), 4, new RepRange(8, 12));

        act.Should().Throw<DomainNotFoundException>();
    }

    [Fact]
    public void AddExerciseToSplit_WithDuplicateExercise_ShouldThrowBusinessRuleViolationException()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);
        var exerciseId = Guid.NewGuid();
        program.AddExerciseToSplit(splitId, exerciseId, 4, new RepRange(8, 12));
        program.ClearDomainEvents();

        var act = () => program.AddExerciseToSplit(splitId, exerciseId, 3, new RepRange(6, 10));

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*already part of split*");
    }

    #endregion

    #region UpdateExerciseInSplit

    [Fact]
    public void UpdateExerciseInSplit_ShouldUpdateAndRaiseEvent()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);
        var exerciseId = Guid.NewGuid();
        var exercise = program.AddExerciseToSplit(splitId, exerciseId, 4, new RepRange(8, 12));
        program.ClearDomainEvents();

        program.UpdateExerciseInSplit(splitId, exercise.Id, 5, new RepRange(6, 10));

        var updated = program.Splits.Single().Exercises.Single();
        updated.Sets.Should().Be(5);
        updated.RepRange.Minimum.Should().Be(6);
        updated.RepRange.Maximum.Should().Be(10);
        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SplitExerciseChangedEvent>();
    }

    [Fact]
    public void UpdateExerciseInSplit_WithInvalidSplitId_ShouldThrowDomainNotFoundException()
    {
        var program = CreateDefaultProgram();

        var act = () => program.UpdateExerciseInSplit(Guid.NewGuid(), Guid.NewGuid(), 5, new RepRange(6, 10));

        act.Should().Throw<DomainNotFoundException>();
    }

    #endregion

    #region RemoveExerciseFromSplit

    [Fact]
    public void RemoveExerciseFromSplit_ShouldRemoveAndRaiseEvent()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);
        var exerciseId = Guid.NewGuid();
        var exercise = program.AddExerciseToSplit(splitId, exerciseId, 4, new RepRange(8, 12));
        program.ClearDomainEvents();

        program.RemoveExerciseFromSplit(splitId, exercise.Id);

        program.Splits.Single().Exercises.Should().BeEmpty();
        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SplitExerciseChangedEvent>();
    }

    [Fact]
    public void RemoveExerciseFromSplit_WithInvalidSplitId_ShouldThrowDomainNotFoundException()
    {
        var program = CreateDefaultProgram();

        var act = () => program.RemoveExerciseFromSplit(Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainNotFoundException>();
    }

    #endregion

    #region ActivateSplitExercise

    [Fact]
    public void ActivateSplitExercise_ShouldActivateExerciseAndRaiseEvent()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);
        var split = program.Splits.Single();
        program.ActivateSplit(splitId);
        var exercise = program.AddExerciseToSplit(splitId, Guid.NewGuid(), 4, new RepRange(8, 12));
        program.ClearDomainEvents();

        program.ActivateSplitExercise(splitId, exercise.Id);

        var activatedExercise = program.Splits.Single().Exercises.Single();
        activatedExercise.IsActive.Should().BeTrue();
        activatedExercise.IsDeleted.Should().BeFalse();
        program.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SplitExerciseChangedEvent>();
    }

    [Fact]
    public void ActivateSplitExercise_WhenProgramIsInactive_ShouldThrowBusinessRuleViolationException()
    {
        var program = CreateDefaultProgram();
        var split = program.AddSplit("Push Day", 1);

        var act = () => program.ActivateSplitExercise(split.Id, Guid.NewGuid());

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*not active*");
    }

    [Fact]
    public void ActivateSplitExercise_WhenSplitIsInactive_ShouldThrowBusinessRuleViolationException()
    {
        var program = CreateDefaultProgram();
        program.Activate();
        var split = program.AddSplit("Push Day", 1);
        program.ClearDomainEvents();

        var act = () => program.ActivateSplitExercise(split.Id, Guid.NewGuid());

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*not active*");
    }

    #endregion

    #region ContainsExercise

    [Fact]
    public void ContainsExercise_WhenExerciseExists_ShouldReturnTrue()
    {
        var program = CreateActiveProgramWithSplit(out var splitId);
        var exerciseId = Guid.NewGuid();
        program.AddExerciseToSplit(splitId, exerciseId, 4, new RepRange(8, 12));

        program.ContainsExercise(exerciseId).Should().BeTrue();
    }

    [Fact]
    public void ContainsExercise_WhenExerciseDoesNotExist_ShouldReturnFalse()
    {
        var program = CreateActiveProgramWithSplit(out _);

        program.ContainsExercise(Guid.NewGuid()).Should().BeFalse();
    }

    #endregion
}
