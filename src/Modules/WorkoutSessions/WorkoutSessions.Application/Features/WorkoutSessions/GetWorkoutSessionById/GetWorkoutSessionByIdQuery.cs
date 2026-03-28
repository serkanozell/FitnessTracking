using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionById
{
    public sealed record GetWorkoutSessionByIdQuery(Guid Id) : IQuery<Result<WorkoutSessionDetailDto>>;
}