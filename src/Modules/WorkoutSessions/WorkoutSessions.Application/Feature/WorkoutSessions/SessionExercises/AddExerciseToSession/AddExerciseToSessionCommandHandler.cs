using WorkoutPrograms.Application.Services;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    internal sealed class AddExerciseToSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutProgramReadService _workoutProgramReadService,
        IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<AddExerciseToSessionCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddExerciseToSessionCommand request, CancellationToken cancellationToken)
        {
            var workoutSession = await _workoutSessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);

            if (workoutSession is null)
                return WorkoutSessionErrors.NotFound(request.WorkoutSessionId);

            var workoutProgram = await _workoutProgramReadService.GetWorkoutProgramByIdAsync(workoutSession.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
                return WorkoutSessionErrors.ProgramNotFound(workoutSession.WorkoutProgramId);

            if (!workoutProgram.ContainsExercise(request.ExerciseId))
                return WorkoutSessionErrors.ExerciseNotInProgram(request.ExerciseId, workoutProgram.Id);

            var programExercise = workoutProgram.Splits
                                                .SelectMany(s => s.Exercises)
                                                .FirstOrDefault(x => x.ExerciseId == request.ExerciseId);

            var currentSetCount = workoutSession.SessionExercises.Count(x => x.ExerciseId == request.ExerciseId);

            if (currentSetCount >= programExercise!.Sets)
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