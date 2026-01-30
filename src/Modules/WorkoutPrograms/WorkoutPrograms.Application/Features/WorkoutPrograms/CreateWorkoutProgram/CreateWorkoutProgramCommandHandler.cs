using WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;

internal sealed class CreateWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<CreateWorkoutProgramCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkoutProgramCommand request, CancellationToken cancellationToken)
    {
        var workoutProgram = WorkoutProgram.Create(request.Name,
                                            request.StartDate,
                                            request.EndDate);

        await _workoutProgramRepository.AddAsync(workoutProgram, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return workoutProgram.Id;
    }
}