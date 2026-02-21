using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions
{
    internal sealed class GetWorkoutSessionsQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetWorkoutSessionsQuery, Result<IReadOnlyList<WorkoutSessionDto>>>
    {
        public async Task<Result<IReadOnlyList<WorkoutSessionDto>>> Handle(GetWorkoutSessionsQuery request, CancellationToken cancellationToken)
        {
            var workoutSessions = await _workoutSessionRepository.GetAllAsync(cancellationToken);

            return workoutSessions.Select(WorkoutSessionDto.FromEntity)
                                                  .ToList();
        }
    }
}