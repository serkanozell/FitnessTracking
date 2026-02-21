using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession
{
    internal sealed class RemoveExerciseFromSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<RemoveExerciseFromSessionCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RemoveExerciseFromSessionCommand request, CancellationToken cancellationToken)
        {
            var workoutSession = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (workoutSession is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            var entry = workoutSession.SessionExercises.FirstOrDefault(x => x.Id == request.SessionExerciseId);

            if (entry is null)
                return WorkoutSessionErrors.SessionExerciseNotFound(request.WorkoutSessionId, request.SessionExerciseId);

            workoutSession.RemoveEntry(request.SessionExerciseId);

            await _workoutSessionRepository.UpdateAsync(workoutSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}