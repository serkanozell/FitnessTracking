using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Exercises.Domain.UnitTests;

public class ExerciseTests
{
    private const string DefaultName = "Bench Press";
    private const MuscleGroup DefaultPrimary = MuscleGroup.Chest;
    private const MuscleGroup DefaultSecondary = MuscleGroup.Triceps;
    private const string DefaultDescription = "Flat barbell bench press";

    private static Exercise CreateDefaultExercise()
        => Exercise.Create(DefaultName, DefaultPrimary, DefaultSecondary, DefaultDescription);

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var exercise = Exercise.Create(DefaultName, DefaultPrimary, DefaultSecondary, DefaultDescription);

        exercise.Id.Should().NotBeEmpty();
        exercise.Name.Should().Be(DefaultName);
        exercise.PrimaryMuscleGroup.Should().Be(DefaultPrimary);
        exercise.SecondaryMuscleGroup.Should().Be(DefaultSecondary);
        exercise.Description.Should().Be(DefaultDescription);
    }

    [Fact]
    public void Create_ShouldRaiseExerciseCreatedEvent()
    {
        var exercise = Exercise.Create(DefaultName, DefaultPrimary, DefaultSecondary, DefaultDescription);

        exercise.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ExerciseCreatedEvent>()
            .Which.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void Create_WithoutSecondaryMuscleGroup_ShouldSetNullSecondary()
    {
        var exercise = Exercise.Create(DefaultName, DefaultPrimary, null, DefaultDescription);

        exercise.SecondaryMuscleGroup.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var exercise = CreateDefaultExercise();

        exercise.Update("Incline Press", MuscleGroup.Shoulders, MuscleGroup.Chest, "Incline barbell press", null, null);

        exercise.Name.Should().Be("Incline Press");
        exercise.PrimaryMuscleGroup.Should().Be(MuscleGroup.Shoulders);
        exercise.SecondaryMuscleGroup.Should().Be(MuscleGroup.Chest);
        exercise.Description.Should().Be("Incline barbell press");
    }

    [Fact]
    public void Update_ShouldRaiseExerciseUpdatedEvent()
    {
        var exercise = CreateDefaultExercise();
        exercise.ClearDomainEvents();

        exercise.Update("Incline Press", MuscleGroup.Shoulders, null, "Incline barbell press", null, null);

        exercise.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ExerciseUpdatedEvent>()
            .Which.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrueAndIsDeletedFalse()
    {
        var exercise = CreateDefaultExercise();
        exercise.Delete();
        exercise.ClearDomainEvents();

        exercise.Activate();

        exercise.IsActive.Should().BeTrue();
        exercise.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldRaiseExerciseActivatedEvent()
    {
        var exercise = CreateDefaultExercise();
        exercise.ClearDomainEvents();

        exercise.Activate();

        exercise.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ExerciseActivatedEvent>()
            .Which.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void Delete_ShouldSetIsActiveFalseAndIsDeletedTrue()
    {
        var exercise = CreateDefaultExercise();
        exercise.Activate();
        exercise.ClearDomainEvents();

        exercise.Delete();

        exercise.IsActive.Should().BeFalse();
        exercise.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_ShouldRaiseExerciseDeletedEvent()
    {
        var exercise = CreateDefaultExercise();
        exercise.ClearDomainEvents();

        exercise.Delete();

        exercise.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ExerciseDeletedEvent>()
            .Which.ExerciseId.Should().Be(exercise.Id);
    }
}
