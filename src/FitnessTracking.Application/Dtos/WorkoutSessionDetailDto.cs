using FitnessTracking.Domain.Entity;

public sealed class WorkoutSessionDetailDto
{
    public Guid Id { get; init; }
    public Guid WorkoutProgramId { get; init; }
    public DateTime Date { get; init; }
    public IReadOnlyList<WorkoutExerciseDto> Exercises { get; init; } = Array.Empty<WorkoutExerciseDto>();

    public static WorkoutSessionDetailDto FromEntity(WorkoutSession entity) =>
        new()
        {
            Id = entity.Id,
            WorkoutProgramId = entity.WorkoutProgramId,
            Date = entity.Date,
            Exercises = entity.WorkoutExercises
                .Select(WorkoutExerciseDto.FromEntity)
                .ToArray()
        };
}