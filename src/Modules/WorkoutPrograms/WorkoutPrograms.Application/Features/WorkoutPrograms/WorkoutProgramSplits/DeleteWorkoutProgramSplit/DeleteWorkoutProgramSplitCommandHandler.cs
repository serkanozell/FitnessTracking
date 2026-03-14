using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    internal sealed class DeleteWorkoutProgramSplitCommandHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IWorkoutProgramsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<DeleteWorkoutProgramSplitCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, workoutProgram.UserId);
            if (ownershipError is not null)
                return ownershipError;

            var split = workoutProgram.Splits.SingleOrDefault(s => s.Id == request.SplitId);

            if (split is null)
                return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.SplitId);

            workoutProgram.RemoveSplit(request.SplitId);

            _workoutProgramRepository.Update(workoutProgram);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}