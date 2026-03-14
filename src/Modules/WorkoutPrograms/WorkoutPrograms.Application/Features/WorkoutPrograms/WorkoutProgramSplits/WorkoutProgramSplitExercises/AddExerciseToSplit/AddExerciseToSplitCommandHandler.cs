using WorkoutPrograms.Domain.Repositories;
using WorkoutPrograms.Domain.ValueObjects;

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

            var repRange = new RepRange(request.MinimumReps, request.MaximumReps);
            var splitExercise = workoutProgram.AddExerciseToSplit(request.WorkoutProgramSplitId,
                                                                  request.ExerciseId,
                                                                  request.Sets,
                                                                  repRange);

            _workoutProgramRepository.Update(workoutProgram);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return splitExercise.Id;
        }
    }
}