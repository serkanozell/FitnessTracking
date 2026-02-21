using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;

internal sealed class ActivateSplitExerciseCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<ActivateSplitExerciseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ActivateSplitExerciseCommand request, CancellationToken cancellationToken)
    {
        var workoutProgram = await _workoutProgramRepository.GetByIdWithExercisesAsync(request.WorkoutProgramId, cancellationToken);

        if (workoutProgram is null)
            return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

        if (!workoutProgram.IsActive)
            return WorkoutProgramErrors.NotActive(request.WorkoutProgramId);

        var workoutProgramSplit = workoutProgram.Splits.SingleOrDefault(s => s.Id == request.SplitId);

        if (workoutProgramSplit is null)
            return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.SplitId);

        var splitExercise = workoutProgramSplit.Exercises.SingleOrDefault(e => e.Id == request.WorkoutSplitExerciseId);

        if (splitExercise is null)
            return WorkoutProgramErrors.ExerciseNotFoundInSplit(request.SplitId, request.WorkoutSplitExerciseId);

        workoutProgram.ActivateSplitExercise(request.SplitId, request.WorkoutSplitExerciseId);

        await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return splitExercise.Id;
    }
}