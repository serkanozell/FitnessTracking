using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionById
{
    public sealed record GetWorkoutSessionByIdQuery(Guid Id) : IQuery<Result<WorkoutSessionDetailDto>>;
}