using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Application.Errors;
using WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionById;

internal sealed class GetWorkoutSessionByIdQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetWorkoutSessionByIdQuery, Result<WorkoutSessionDetailDto>>
{
    public async Task<Result<WorkoutSessionDetailDto>> Handle(GetWorkoutSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return WorkoutSessionErrors.NotFound(request.Id);

        return WorkoutSessionDetailDto.FromEntity(session);
    }
}