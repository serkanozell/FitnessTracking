using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise
{
    internal sealed class UpdateSplitExerciseCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<UpdateSplitExerciseCommand, bool>
    {
        public async Task<bool> Handle(
            UpdateSplitExerciseCommand request,
            CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);
            if (workoutProgram is null)
            {
                return false;
            }

            var split = workoutProgram.Splits.SingleOrDefault(x => x.Id == request.WorkoutProgramSplitId);

            if (split is null)
            {
                return false;
            }

            var exercise = split.Exercises.SingleOrDefault(x => x.Id == request.WorkoutProgramExerciseId);

            if (exercise is null)
            {
                return false;
            }

            // Aggregate içinden entity güncelle
            split.UpdateExercise(request.WorkoutProgramExerciseId,
                                 request.Sets,
                                 request.TargetReps);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}