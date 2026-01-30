using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    internal sealed class RemoveSplitExerciseCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<RemoveSplitExerciseCommand, bool>
    {
        public async Task<bool> Handle(RemoveSplitExerciseCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdWithExercisesAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null || workoutProgram.Splits is null)
            {
                return false;
            }

            var split = workoutProgram.Splits.FirstOrDefault(s => s.Id == request.WorkoutProgramSplitId);
            if (split is null)
            {
                return false;
            }

            var exerciseToRemove = split.Exercises.FirstOrDefault(e => e.Id == request.WorkoutProgramExerciseId);

            if (exerciseToRemove is null)
            {
                return false;
            }

            split.Exercises.Remove(exerciseToRemove);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}