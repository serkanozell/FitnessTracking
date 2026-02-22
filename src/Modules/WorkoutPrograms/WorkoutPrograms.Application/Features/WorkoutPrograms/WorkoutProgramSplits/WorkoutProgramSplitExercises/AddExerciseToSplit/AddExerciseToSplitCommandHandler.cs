using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    internal sealed class AddExerciseToSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<AddExerciseToSplitCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddExerciseToSplitCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdWithExercisesAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var programSplit = workoutProgram.Splits.SingleOrDefault(s => s.Id == request.WorkoutProgramSplitId);

            if (programSplit is null)
                return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.WorkoutProgramSplitId);

            var splitExercise = workoutProgram.AddExerciseToSplit(request.WorkoutProgramSplitId,
                                                                  request.ExerciseId,
                                                                  request.Sets,
                                                                  request.MinimumReps,
                                                                  request.MaximumReps);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return splitExercise.Id;
        }
    }
}