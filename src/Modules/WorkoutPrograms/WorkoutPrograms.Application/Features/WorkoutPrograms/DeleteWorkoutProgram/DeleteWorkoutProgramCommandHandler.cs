using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    internal sealed class DeleteWorkoutProgramCommandHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IWorkoutProgramsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<DeleteWorkoutProgramCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, program.UserId);
            if (ownershipError is not null)
                return ownershipError;

            program.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}