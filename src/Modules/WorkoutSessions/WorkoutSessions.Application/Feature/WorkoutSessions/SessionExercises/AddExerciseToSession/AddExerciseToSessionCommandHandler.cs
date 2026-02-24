using WorkoutPrograms.Contracts;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    internal sealed class AddExerciseToSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutProgramModule _workoutProgramModule,
        IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<AddExerciseToSessionCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddExerciseToSessionCommand request, CancellationToken cancellationToken)
        {
            var workoutSession = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (workoutSession is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            if (!await _workoutProgramModule.ExistsAsync(workoutSession.WorkoutProgramId, cancellationToken))
                return WorkoutSessionErrors.ProgramNotFound(workoutSession.WorkoutProgramId);

            var programExercise = await _workoutProgramModule.GetProgramExerciseAsync(workoutSession.WorkoutProgramId, request.ExerciseId, cancellationToken);

            if (programExercise is null)
                return WorkoutSessionErrors.ExerciseNotInProgram(request.ExerciseId, workoutSession.WorkoutProgramId);

            var currentSetCount = workoutSession.SessionExercises.Count(x => x.ExerciseId == request.ExerciseId);

            if (currentSetCount >= programExercise.Sets)
                return WorkoutSessionErrors.SetLimitExceeded(request.ExerciseId, programExercise.Sets, currentSetCount);

            if (request.SetNumber > programExercise.Sets)
                return WorkoutSessionErrors.SetNumberExceedsLimit(request.ExerciseId, programExercise.Sets);

            if (workoutSession.SessionExercises.Any(x => x.ExerciseId == request.ExerciseId && x.SetNumber == request.SetNumber))
                return WorkoutSessionErrors.DuplicateSet(request.ExerciseId, request.SetNumber);

            var entry = workoutSession.AddEntry(request.ExerciseId, request.SetNumber, request.Weight, request.Reps);

            await _workoutSessionRepository.UpdateAsync(workoutSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entry.Id;
        }
    }
}