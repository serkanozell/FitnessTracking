using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionById;

internal sealed class GetWorkoutSessionByIdQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetWorkoutSessionByIdQuery, WorkoutSessionDetailDto?>
{
    public async Task<WorkoutSessionDetailDto?> Handle(GetWorkoutSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            return null;
        }
        return WorkoutSessionDetailDto.FromEntity(session);
    }
}