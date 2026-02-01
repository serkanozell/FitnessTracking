using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.GetExercisesBySession
{
    internal sealed class GetExercisesBySessionQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetExercisesBySessionQuery, IReadOnlyList<SessionExerciseDto>>
    {
        public async Task<IReadOnlyList<SessionExerciseDto>> Handle(GetExercisesBySessionQuery request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);
            if (session is null)
            {
                return Array.Empty<SessionExerciseDto>();
            }

            return session.SessionExercises
                .Select(SessionExerciseDto.FromEntity)
                .ToArray();
        }
    }
}