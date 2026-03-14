using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram
{
    internal sealed class UpdateWorkoutProgramCommandHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IWorkoutProgramsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<UpdateWorkoutProgramCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, workoutProgram.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (request.EndDate <= request.StartDate)
                return WorkoutProgramErrors.InvalidDateRange();

            workoutProgram.Update(request.Name, request.StartDate, request.EndDate);

            _workoutProgramRepository.Update(workoutProgram);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}