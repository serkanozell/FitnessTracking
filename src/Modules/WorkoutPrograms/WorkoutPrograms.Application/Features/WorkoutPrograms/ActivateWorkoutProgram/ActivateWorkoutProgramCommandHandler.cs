using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram
{
    internal sealed class ActivateWorkoutProgramCommandHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IWorkoutProgramsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<ActivateWorkoutProgramCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, workoutProgram.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (workoutProgram.IsActive)
                return WorkoutProgramErrors.AlreadyActive(request.Id);

            workoutProgram.Activate();

            _workoutProgramRepository.Update(workoutProgram);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return workoutProgram.Id;
        }
    }
}