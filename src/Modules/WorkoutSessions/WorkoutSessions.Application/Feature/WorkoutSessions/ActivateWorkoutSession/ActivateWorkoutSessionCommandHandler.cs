using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.ActivateWorkoutSession
{
    internal sealed class ActivateWorkoutSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<ActivateWorkoutSessionCommand, Guid>
    {
        public async Task<Guid> Handle(ActivateWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            WorkoutSession session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutSession ({request.WorkoutSessionId}) not found.");

            session.Activate();

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}