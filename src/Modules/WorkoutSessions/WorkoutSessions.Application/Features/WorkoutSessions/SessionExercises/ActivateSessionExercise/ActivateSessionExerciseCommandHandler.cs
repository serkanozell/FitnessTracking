using BuildingBlocks.Application.Abstractions;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.ActivateSessionExercise
{
    internal sealed class ActivateSessionExerciseCommandHandler(
        IWorkoutSessionRepository _workoutSessionRepository,
        IWorkoutSessionsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<ActivateSessionExerciseCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateSessionExerciseCommand request, CancellationToken cancellationToken)
        {
            var workoutSession = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (workoutSession is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, workoutSession.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (!workoutSession.IsActive)
                return WorkoutSessionErrors.NotActive(request.WorkoutSessionId);

            var entry = workoutSession.SessionExercises.FirstOrDefault(x => x.Id == request.SessionExerciseId);

            if (entry is null)
                return WorkoutSessionErrors.SessionExerciseNotFound(request.WorkoutSessionId, request.SessionExerciseId);

            workoutSession.ActivateEntry(request.SessionExerciseId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.SessionExerciseId;
        }
    }
}