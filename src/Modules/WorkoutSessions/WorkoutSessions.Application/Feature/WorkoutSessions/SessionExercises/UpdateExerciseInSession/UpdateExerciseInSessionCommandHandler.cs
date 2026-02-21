using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.UpdateExerciseInSession
{
    internal sealed class UpdateExerciseInSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<UpdateExerciseInSessionCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateExerciseInSessionCommand request, CancellationToken cancellationToken)
        {
            var workoutSession = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (workoutSession is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            var sessionExercise = workoutSession.SessionExercises.FirstOrDefault(x => x.Id == request.SessionExerciseId);

            if (sessionExercise is null)
                return WorkoutSessionErrors.SessionExerciseNotFound(request.WorkoutSessionId, request.SessionExerciseId);

            workoutSession.UpdateEntry(request.SessionExerciseId, request.SetNumber, request.Weight, request.Reps);

            await _workoutSessionRepository.UpdateAsync(workoutSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}