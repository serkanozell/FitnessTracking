using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.UpdateExerciseInSession
{
    internal sealed class UpdateExerciseInSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<UpdateExerciseInSessionCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateExerciseInSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutSession ({request.WorkoutSessionId}) not found.");

            session.UpdateEntry(request.SessionExerciseId,
                                request.SetNumber,
                                request.Weight,
                                request.Reps);

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}