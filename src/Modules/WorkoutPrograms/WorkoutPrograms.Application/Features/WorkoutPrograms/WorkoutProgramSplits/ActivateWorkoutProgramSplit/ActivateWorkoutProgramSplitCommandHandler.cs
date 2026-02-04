using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;

internal sealed class ActivateWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<ActivateWorkoutProgramSplitCommand, Guid>
{
    public async Task<Guid> Handle(ActivateWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
    {
        // Splits are part of aggregate, so load aggregate and mutate inside.
        WorkoutProgram? program = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutProgram ({request.WorkoutProgramId}) not found.");
        
        program.ActivateSplit(request.SplitId);

        await _workoutProgramRepository.UpdateAsync(program, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return request.SplitId;
    }
}