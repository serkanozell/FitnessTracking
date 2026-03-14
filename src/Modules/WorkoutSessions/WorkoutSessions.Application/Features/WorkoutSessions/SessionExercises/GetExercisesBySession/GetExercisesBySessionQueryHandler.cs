using BuildingBlocks.Application.Abstractions;
using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.GetExercisesBySession
{
    internal sealed class GetExercisesBySessionQueryHandler(
        IWorkoutSessionRepository _workoutSessionRepository,
        ICurrentUser _currentUser) : IQueryHandler<GetExercisesBySessionQuery, Result<IReadOnlyList<SessionExerciseDto>>>
    {
        public async Task<Result<IReadOnlyList<SessionExerciseDto>>> Handle(GetExercisesBySessionQuery request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (session is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, session.UserId);
            if (ownershipError is not null)
                return ownershipError;

            return session.SessionExercises
                                           .Select(SessionExerciseDto.FromEntity)
                                           .ToList();
        }
    }
}