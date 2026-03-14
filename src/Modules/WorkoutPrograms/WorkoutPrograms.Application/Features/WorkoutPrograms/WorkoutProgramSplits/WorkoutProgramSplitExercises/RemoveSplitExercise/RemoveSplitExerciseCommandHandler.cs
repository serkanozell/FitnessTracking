using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    internal sealed class RemoveSplitExerciseCommandHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IWorkoutProgramsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<RemoveSplitExerciseCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RemoveSplitExerciseCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdWithExercisesAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, workoutProgram.UserId);
            if (ownershipError is not null)
                return ownershipError;

            var programSplit = workoutProgram.Splits.SingleOrDefault(s => s.Id == request.WorkoutProgramSplitId);

            if (programSplit is null)
                return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.WorkoutProgramSplitId);

            var splitExercise = programSplit.Exercises.SingleOrDefault(e => e.Id == request.WorkoutProgramExerciseId);

            if (splitExercise is null)
                return WorkoutProgramErrors.ExerciseNotFoundInSplit(request.WorkoutProgramSplitId, request.WorkoutProgramExerciseId);

            workoutProgram.RemoveExerciseFromSplit(request.WorkoutProgramSplitId, request.WorkoutProgramExerciseId);

            _workoutProgramRepository.Update(workoutProgram);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}