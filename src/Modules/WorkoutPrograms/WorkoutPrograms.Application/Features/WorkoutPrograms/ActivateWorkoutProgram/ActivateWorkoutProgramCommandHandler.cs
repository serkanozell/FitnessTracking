using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram;

internal sealed class ActivateWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<ActivateWorkoutProgramCommand, Guid>
{
    public async Task<Guid> Handle(ActivateWorkoutProgramCommand request, CancellationToken cancellationToken)
    {
        WorkoutProgram? program = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutProgram ({request.Id}) not found.");

        program.Activate();

        await _workoutProgramRepository.UpdateAsync(program, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return program.Id;
    }
}