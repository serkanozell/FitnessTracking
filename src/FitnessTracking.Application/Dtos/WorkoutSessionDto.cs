using FitnessTracking.Domain.Entity;

public sealed class WorkoutSessionDto
{
    public Guid Id { get; init; }
    public Guid WorkoutProgramId { get; init; }
    public DateTime Date { get; init; }

    public static WorkoutSessionDto FromEntity(WorkoutSession entity) =>
        new()
        {
            Id = entity.Id,
            WorkoutProgramId = entity.WorkoutProgramId,
            Date = entity.Date
        };
}