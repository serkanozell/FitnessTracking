using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession
{
    internal sealed class DeleteWorkoutSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<DeleteWorkoutSessionCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (session is null)
                return WorkoutSessionErrors.NotFound(request.Id);

            await _workoutSessionRepository.DeleteAsync(request.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}