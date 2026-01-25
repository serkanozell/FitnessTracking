public sealed class WorkoutSessionDetailsDto
{
    public Guid Id { get; init; }
    public Guid WorkoutProgramId { get; init; }
    public DateTime Date { get; init; }
    public IReadOnlyList<WorkoutExerciseDto> Exercises { get; init; } = Array.Empty<WorkoutExerciseDto>();
}