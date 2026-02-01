using WorkoutSessions.Application.Dtos;

public sealed class GetWorkoutSessionsByProgramQuery : IQuery<IReadOnlyList<WorkoutSessionDto>>
{
    public Guid WorkoutProgramId { get; init; }
}