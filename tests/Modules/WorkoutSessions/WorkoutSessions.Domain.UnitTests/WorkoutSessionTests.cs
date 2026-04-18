using BuildingBlocks.Domain.Exceptions;
using FluentAssertions;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Events;
using Xunit;

namespace WorkoutSessions.Domain.UnitTests;

public class WorkoutSessionTests
{
    private static readonly Guid DefaultProgramId = Guid.NewGuid();
    private static readonly DateTime DefaultDate = new(2025, 6, 15);

    private static WorkoutSession CreateDefaultSession()
        => WorkoutSession.Create(Guid.NewGuid(), DefaultProgramId, Guid.NewGuid(), DefaultDate);

    private static WorkoutSession CreateActiveSessionWithEntry(out Guid entryId)
    {
        var session = CreateDefaultSession();
        session.Activate();
        var entry = session.AddEntry(Guid.NewGuid(), 1, 80m, 10);
        entryId = entry.Id;
        session.ClearDomainEvents();
        return session;
    }

    #region Create

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DefaultProgramId, Guid.NewGuid(), DefaultDate);

        session.Id.Should().NotBeEmpty();
        session.WorkoutProgramId.Should().Be(DefaultProgramId);
        session.Date.Should().Be(DefaultDate);
        session.SessionExercises.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldRaiseWorkoutSessionCreatedEvent()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DefaultProgramId, Guid.NewGuid(), DefaultDate);

        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutSessionCreatedEvent>()
            .Which.Should().Match<WorkoutSessionCreatedEvent>(e =>
                e.SessionId == session.Id && e.ProgramId == DefaultProgramId);
    }

    #endregion

    #region Activate / Delete

    [Fact]
    public void Activate_ShouldSetIsActiveTrueAndIsDeletedFalse()
    {
        var session = CreateDefaultSession();
        session.Delete();
        session.ClearDomainEvents();

        session.Activate();

        session.IsActive.Should().BeTrue();
        session.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldRaiseWorkoutSessionActivatedEvent()
    {
        var session = CreateDefaultSession();
        session.ClearDomainEvents();

        session.Activate();

        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutSessionActivatedEvent>()
            .Which.SessionId.Should().Be(session.Id);
    }

    [Fact]
    public void Delete_ShouldSetIsActiveFalseAndIsDeletedTrue()
    {
        var session = CreateDefaultSession();
        session.Activate();
        session.ClearDomainEvents();

        session.Delete();

        session.IsActive.Should().BeFalse();
        session.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_ShouldRaiseWorkoutSessionDeletedEvent()
    {
        var session = CreateDefaultSession();
        session.ClearDomainEvents();

        session.Delete();

        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutSessionDeletedEvent>()
            .Which.Should().Match<WorkoutSessionDeletedEvent>(e =>
                e.SessionId == session.Id && e.ProgramId == DefaultProgramId);
    }

    #endregion

    #region UpdateDate

    [Fact]
    public void UpdateDate_ShouldChangeDate()
    {
        var session = CreateDefaultSession();
        var newDate = new DateTime(2025, 7, 1);

        session.UpdateDate(newDate);

        session.Date.Should().Be(newDate);
    }

    [Fact]
    public void UpdateDate_ShouldRaiseWorkoutSessionUpdatedEvent()
    {
        var session = CreateDefaultSession();
        session.ClearDomainEvents();

        session.UpdateDate(new DateTime(2025, 7, 1));

        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkoutSessionUpdatedEvent>()
            .Which.Should().Match<WorkoutSessionUpdatedEvent>(e =>
                e.SessionId == session.Id && e.ProgramId == DefaultProgramId);
    }

    #endregion

    #region AddEntry

    [Fact]
    public void AddEntry_ShouldAddSessionExercise()
    {
        var session = CreateDefaultSession();
        session.ClearDomainEvents();
        var exerciseId = Guid.NewGuid();

        var entry = session.AddEntry(exerciseId, 1, 100m, 8);

        entry.Id.Should().NotBeEmpty();
        entry.ExerciseId.Should().Be(exerciseId);
        entry.SetNumber.Should().Be(1);
        entry.Weight.Should().Be(100m);
        entry.Reps.Should().Be(8);
        session.SessionExercises.Should().ContainSingle()
            .Which.Id.Should().Be(entry.Id);
    }

    [Fact]
    public void AddEntry_ShouldRaiseSessionExerciseChangedEvent()
    {
        var session = CreateDefaultSession();
        session.ClearDomainEvents();

        session.AddEntry(Guid.NewGuid(), 1, 100m, 8);

        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SessionExerciseChangedEvent>()
            .Which.SessionId.Should().Be(session.Id);
    }

    [Fact]
    public void AddEntry_WithDuplicateSet_ShouldThrowBusinessRuleViolationException()
    {
        var session = CreateDefaultSession();
        var exerciseId = Guid.NewGuid();
        session.AddEntry(exerciseId, 1, 100m, 8);

        var act = () => session.AddEntry(exerciseId, 1, 110m, 6);

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Set 1*already exists*");
    }

    [Fact]
    public void AddEntry_SameExerciseDifferentSet_ShouldSucceed()
    {
        var session = CreateDefaultSession();
        var exerciseId = Guid.NewGuid();
        session.AddEntry(exerciseId, 1, 100m, 8);

        var entry2 = session.AddEntry(exerciseId, 2, 100m, 7);

        session.SessionExercises.Should().HaveCount(2);
        entry2.SetNumber.Should().Be(2);
    }

    #endregion

    #region ActivateEntry

    [Fact]
    public void ActivateEntry_ShouldActivateAndRaiseEvent()
    {
        var session = CreateActiveSessionWithEntry(out var entryId);

        session.ActivateEntry(entryId);

        var entry = session.SessionExercises.Single(e => e.Id == entryId);
        entry.IsActive.Should().BeTrue();
        entry.IsDeleted.Should().BeFalse();
        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SessionExerciseChangedEvent>();
    }

    [Fact]
    public void ActivateEntry_WhenSessionIsInactive_ShouldThrowBusinessRuleViolationException()
    {
        var session = CreateDefaultSession();
        var entry = session.AddEntry(Guid.NewGuid(), 1, 80m, 10);

        var act = () => session.ActivateEntry(entry.Id);

        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*not active*");
    }

    [Fact]
    public void ActivateEntry_WhenEntryNotFound_ShouldThrowDomainNotFoundException()
    {
        var session = CreateDefaultSession();
        session.Activate();
        session.ClearDomainEvents();

        var act = () => session.ActivateEntry(Guid.NewGuid());

        act.Should().Throw<DomainNotFoundException>();
    }

    #endregion

    #region UpdateEntry

    [Fact]
    public void UpdateEntry_ShouldUpdatePropertiesAndRaiseEvent()
    {
        var session = CreateActiveSessionWithEntry(out var entryId);

        session.UpdateEntry(entryId, 2, 90m, 12);

        var entry = session.SessionExercises.Single(e => e.Id == entryId);
        entry.SetNumber.Should().Be(2);
        entry.Weight.Should().Be(90m);
        entry.Reps.Should().Be(12);
        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SessionExerciseChangedEvent>();
    }

    [Fact]
    public void UpdateEntry_WhenEntryNotFound_ShouldThrowDomainNotFoundException()
    {
        var session = CreateDefaultSession();

        var act = () => session.UpdateEntry(Guid.NewGuid(), 2, 90m, 12);

        act.Should().Throw<DomainNotFoundException>();
    }

    #endregion

    #region RemoveEntry (by Id)

    [Fact]
    public void RemoveEntryById_ShouldRemoveAndRaiseEvent()
    {
        var session = CreateActiveSessionWithEntry(out var entryId);

        session.RemoveEntry(entryId);

        session.SessionExercises.Should().BeEmpty();
        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SessionExerciseChangedEvent>();
    }

    [Fact]
    public void RemoveEntryById_WhenNotFound_ShouldNotThrowAndNotRaiseEvent()
    {
        var session = CreateDefaultSession();
        session.ClearDomainEvents();

        var act = () => session.RemoveEntry(Guid.NewGuid());

        act.Should().NotThrow();
        session.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region RemoveEntry (by ExerciseId + SetNumber)

    [Fact]
    public void RemoveEntryByExerciseAndSet_ShouldRemoveAndRaiseEvent()
    {
        var session = CreateDefaultSession();
        var exerciseId = Guid.NewGuid();
        session.AddEntry(exerciseId, 1, 100m, 8);
        session.ClearDomainEvents();

        session.RemoveEntry(exerciseId, 1);

        session.SessionExercises.Should().BeEmpty();
        session.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<SessionExerciseChangedEvent>();
    }

    [Fact]
    public void RemoveEntryByExerciseAndSet_WhenNotFound_ShouldNotThrowAndNotRaiseEvent()
    {
        var session = CreateDefaultSession();
        session.ClearDomainEvents();

        var act = () => session.RemoveEntry(Guid.NewGuid(), 99);

        act.Should().NotThrow();
        session.DomainEvents.Should().BeEmpty();
    }

    #endregion
}
