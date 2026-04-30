using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionDetailView;

public sealed record GetWorkoutSessionDetailViewQuery(Guid Id) : IQuery<Result<WorkoutSessionDetailViewDto>>;
