using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram
{
    internal sealed class ActivateWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<ActivateWorkoutProgramCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.Id);

            if (workoutProgram.IsActive)
                return WorkoutProgramErrors.AlreadyActive(request.Id);

            workoutProgram.Activate();

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return workoutProgram.Id;
        }
    }
}