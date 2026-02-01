using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession
{
    internal sealed class DeleteWorkoutSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<DeleteWorkoutSessionCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (session is null)
            {
                return Unit.Value; // idempotent
            }

            await _workoutSessionRepository.DeleteAsync(request.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}