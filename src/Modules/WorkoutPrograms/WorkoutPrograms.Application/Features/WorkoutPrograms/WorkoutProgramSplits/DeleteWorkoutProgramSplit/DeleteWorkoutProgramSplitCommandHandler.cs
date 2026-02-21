using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    internal sealed class DeleteWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<DeleteWorkoutProgramSplitCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var split = workoutProgram.Splits.SingleOrDefault(s => s.Id == request.SplitId);

            if (split is null)
                return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.SplitId);

            workoutProgram.RemoveSplit(request.SplitId);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}