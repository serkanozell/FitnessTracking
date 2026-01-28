using FitnessTracking.Domain.Entity;

public sealed class WorkoutSessionDetailDto
{
    public Guid Id { get; init; }
    public Guid WorkoutProgramId { get; init; }
    public DateTime Date { get; init; }
    public IReadOnlyList<SessionExerciseDto> Exercises { get; init; } = Array.Empty<SessionExerciseDto>();

    public static WorkoutSessionDetailDto FromEntity(WorkoutSession entity) =>
        new()
        {
            Id = entity.Id,
            WorkoutProgramId = entity.WorkoutProgramId,
            Date = entity.Date,
            Exercises = entity.SessionExercises
                .Select(SessionExerciseDto.FromEntity)
                .ToArray()
        };
}