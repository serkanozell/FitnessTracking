using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions
{
    public sealed record GetWorkoutSessionsQuery : IQuery<Result<IReadOnlyList<WorkoutSessionDto>>>;
}