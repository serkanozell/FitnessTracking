using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    internal sealed class DeleteWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<DeleteWorkoutProgramSplitCommand, bool>
    {
        public async Task<bool> Handle(DeleteWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
            {
                return false;
            }

            var split = workoutProgram.Splits.SingleOrDefault(x => x.Id == request.SplitId);

            if (split is null)
            {
                return false;
            }

            workoutProgram.RemoveSplit(request.SplitId);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}