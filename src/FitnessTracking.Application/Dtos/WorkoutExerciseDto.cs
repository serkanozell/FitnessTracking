public sealed class WorkoutExerciseDto
{
    public Guid Id { get; init; }
    public Guid ExerciseId { get; init; }
    public int SetNumber { get; init; }
    public decimal Weight { get; init; }
    public int Reps { get; init; }

    public static WorkoutExerciseDto FromEntity(WorkoutExercise entity) =>
        new()
        {
            Id = entity.Id,
            ExerciseId = entity.ExerciseId,
            SetNumber = entity.SetNumber,
            Weight = entity.Weight,
            Reps = entity.Reps
        };
}