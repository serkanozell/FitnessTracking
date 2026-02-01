using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions
{
    public sealed class GetWorkoutSessionsQuery : IQuery<IReadOnlyList<WorkoutSessionDto>>
    {
    }
}