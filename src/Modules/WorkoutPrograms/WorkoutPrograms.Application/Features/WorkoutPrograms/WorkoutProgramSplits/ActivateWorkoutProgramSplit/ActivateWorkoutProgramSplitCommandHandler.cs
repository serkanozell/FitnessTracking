using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;

internal sealed class ActivateWorkoutProgramSplitCommandHandler(
    IWorkoutProgramRepository _workoutProgramRepository,
    IWorkoutProgramsUnitOfWork _unitOfWork,
    ICurrentUser _currentUser) : ICommandHandler<ActivateWorkoutProgramSplitCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ActivateWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
    {
        var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

        if (workoutProgram is null)
            return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

        var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, workoutProgram.UserId);
        if (ownershipError is not null)
            return ownershipError;

        if (!workoutProgram.IsActive)
            return WorkoutProgramErrors.NotActive(request.WorkoutProgramId);

        var split = workoutProgram.Splits.SingleOrDefault(s => s.Id == request.SplitId);

        if (split is null)
            return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.SplitId);

        workoutProgram.ActivateSplit(request.SplitId);

        _workoutProgramRepository.Update(workoutProgram);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return split.Id;
    }
}