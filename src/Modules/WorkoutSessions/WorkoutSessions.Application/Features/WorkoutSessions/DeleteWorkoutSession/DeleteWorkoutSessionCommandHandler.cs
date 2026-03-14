using BuildingBlocks.Application.Abstractions;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.DeleteWorkoutSession
{
    internal sealed class DeleteWorkoutSessionCommandHandler(
        IWorkoutSessionRepository _workoutSessionRepository,
        IWorkoutSessionsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<DeleteWorkoutSessionCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (session is null)
                return WorkoutSessionErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, session.UserId);
            if (ownershipError is not null)
                return ownershipError;

            session.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}