using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions
{
    internal sealed class GetWorkoutSessionsQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetWorkoutSessionsQuery, IReadOnlyList<WorkoutSessionDto>>
    {
        public async Task<IReadOnlyList<WorkoutSessionDto>> Handle(GetWorkoutSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _workoutSessionRepository.GetAllAsync(cancellationToken);

            if (sessions.Count == 0)
            {
                return Array.Empty<WorkoutSessionDto>();
            }

            var result = sessions
                .Select(s => new WorkoutSessionDto
                {
                    Id = s.Id,
                    WorkoutProgramId = s.WorkoutProgramId,
                    Date = s.Date
                })
                .ToList()
                .AsReadOnly();

            return result;
        }
    }
}