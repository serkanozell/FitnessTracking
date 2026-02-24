namespace Exercises.Contracts;

public record ExerciseInfo(Guid Id, string Name, string PrimaryMuscleGroup, string? SecondaryMuscleGroup, string Description);