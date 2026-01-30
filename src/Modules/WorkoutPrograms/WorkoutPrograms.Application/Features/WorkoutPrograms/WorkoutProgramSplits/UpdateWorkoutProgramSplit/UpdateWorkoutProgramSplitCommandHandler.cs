using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    internal sealed class UpdateWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : IRequestHandler<UpdateWorkoutProgramSplitCommand, bool>
    {
        public async Task<bool> Handle(UpdateWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
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

            workoutProgram.UpdateSplit(request.SplitId, request.Name, request.Order);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}