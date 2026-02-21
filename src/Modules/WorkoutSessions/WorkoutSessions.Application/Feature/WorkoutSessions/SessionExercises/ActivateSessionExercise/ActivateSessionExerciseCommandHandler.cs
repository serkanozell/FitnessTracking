using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.ActivateSessionExercise
{
    internal sealed class ActivateSessionExerciseCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<ActivateSessionExerciseCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateSessionExerciseCommand request, CancellationToken cancellationToken)
        {
            var workoutSession = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (workoutSession is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            if (!workoutSession.IsActive)
                return WorkoutSessionErrors.NotActive(request.WorkoutSessionId);

            var entry = workoutSession.SessionExercises.FirstOrDefault(x => x.Id == request.SessionExerciseId);

            if (entry is null)
                return WorkoutSessionErrors.SessionExerciseNotFound(request.WorkoutSessionId, request.SessionExerciseId);

            workoutSession.ActivateEntry(request.SessionExerciseId);

            await _workoutSessionRepository.UpdateAsync(workoutSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return request.SessionExerciseId;
        }
    }
}