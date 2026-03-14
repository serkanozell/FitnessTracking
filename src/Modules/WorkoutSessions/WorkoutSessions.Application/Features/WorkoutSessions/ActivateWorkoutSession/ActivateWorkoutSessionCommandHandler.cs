using BuildingBlocks.Application.Abstractions;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.ActivateWorkoutSession
{
    internal sealed class ActivateWorkoutSessionCommandHandler(
        IWorkoutSessionRepository _workoutSessionRepository,
        IWorkoutSessionsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<ActivateWorkoutSessionCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (session is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, session.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (session.IsActive)
                return WorkoutSessionErrors.AlreadyActive(request.WorkoutSessionId);

            session.Activate();

            _workoutSessionRepository.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}