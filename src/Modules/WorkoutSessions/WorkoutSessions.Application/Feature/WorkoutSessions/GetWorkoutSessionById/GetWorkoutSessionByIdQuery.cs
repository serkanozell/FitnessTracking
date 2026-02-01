using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionById
{
    public sealed class GetWorkoutSessionByIdQuery : IQuery<WorkoutSessionDetailDto?>
    {
        public Guid Id { get; init; }
    }
}