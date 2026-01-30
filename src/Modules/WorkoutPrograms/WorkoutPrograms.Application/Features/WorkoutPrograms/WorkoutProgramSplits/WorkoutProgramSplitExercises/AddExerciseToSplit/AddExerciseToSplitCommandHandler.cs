using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    internal sealed class AddExerciseToSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : IRequestHandler<AddExerciseToSplitCommand, Guid>
    {
        public async Task<Guid> Handle(AddExerciseToSplitCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
            {
                throw new KeyNotFoundException(
                    $"WorkoutProgram ({request.WorkoutProgramId}) not found.");
            }

            var split = workoutProgram.Splits.SingleOrDefault(x => x.Id == request.WorkoutProgramSplitId);

            if (split is null)
            {
                throw new KeyNotFoundException(
                    $"WorkoutProgramSplit ({request.WorkoutProgramSplitId}) not found in program {request.WorkoutProgramId}.");
            }


            // Entity yönetimi aggregate içinde
            var programExercise = split.AddExercise(request.ExerciseId,
                                                    request.Sets,
                                                    request.TargetReps);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return programExercise.Id;
        }
    }
}