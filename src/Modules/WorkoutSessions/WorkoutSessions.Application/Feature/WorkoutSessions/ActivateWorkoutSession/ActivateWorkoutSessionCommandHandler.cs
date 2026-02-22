using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.ActivateWorkoutSession
{
    internal sealed class ActivateWorkoutSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<ActivateWorkoutSessionCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (session is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            if (session.IsActive)
                return WorkoutSessionErrors.AlreadyActive(request.WorkoutSessionId);

            session.Activate();

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}