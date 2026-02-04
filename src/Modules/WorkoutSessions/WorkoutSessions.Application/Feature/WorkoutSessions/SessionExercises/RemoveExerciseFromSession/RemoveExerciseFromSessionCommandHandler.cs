using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession
{
    public sealed class RemoveExerciseFromSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<RemoveExerciseFromSessionCommand, Unit>
    {
        public async Task<Unit> Handle(
            RemoveExerciseFromSessionCommand request,
            CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);
            if (session is null)
            {
                return Unit.Value;
            }

            session.RemoveEntry(request.SessionExerciseId);

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}