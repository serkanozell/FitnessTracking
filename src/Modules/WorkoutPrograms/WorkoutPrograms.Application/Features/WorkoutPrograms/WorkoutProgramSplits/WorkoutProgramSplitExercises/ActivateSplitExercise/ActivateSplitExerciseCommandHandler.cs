using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;

internal sealed class ActivateSplitExerciseCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<ActivateSplitExerciseCommand, Guid>
{
    public async Task<Guid> Handle(ActivateSplitExerciseCommand request, CancellationToken cancellationToken)
    {
        WorkoutProgram? program = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutProgram ({request.WorkoutProgramId}) not found.");

        program.ActivateSplitExercise(request.SplitId, request.WorkoutSplitExerciseId);

        await _workoutProgramRepository.UpdateAsync(program, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.WorkoutSplitExerciseId;
    }
}