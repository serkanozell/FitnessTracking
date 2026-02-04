using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.ActivateSessionExercise
{
    internal sealed class ActivateSessionExerciseCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<ActivateSessionExerciseCommand, Guid>
    {
        public async Task<Guid> Handle(ActivateSessionExerciseCommand request, CancellationToken cancellationToken)
        {
            WorkoutSession session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutSession ({request.WorkoutSessionId}) not found.");

            session.ActivateEntry(request.SessionExerciseId);

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.SessionExerciseId;
        }
    }
}