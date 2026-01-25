public sealed class WorkoutExerciseDto
{
    public Guid Id { get; init; }
    public Guid ExerciseId { get; init; }
    public int SetNumber { get; init; }
    public decimal Weight { get; init; }
    public int Reps { get; init; }
}