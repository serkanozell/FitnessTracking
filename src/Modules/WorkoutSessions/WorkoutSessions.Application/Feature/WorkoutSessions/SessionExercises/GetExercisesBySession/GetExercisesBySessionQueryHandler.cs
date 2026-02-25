using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.GetExercisesBySession
{
    internal sealed class GetExercisesBySessionQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetExercisesBySessionQuery, Result<IReadOnlyList<SessionExerciseDto>>>
    {
        public async Task<Result<IReadOnlyList<SessionExerciseDto>>> Handle(GetExercisesBySessionQuery request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (session is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            return session.SessionExercises
                                           .Select(SessionExerciseDto.FromEntity)
                                           .ToList();
        }
    }
}