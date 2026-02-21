using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    internal sealed class UpdateWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<UpdateWorkoutProgramSplitCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var split = program.Splits.SingleOrDefault(s => s.Id == request.SplitId);

            if (split is null)
                return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.SplitId);

            program.UpdateSplit(request.SplitId, request.Name, request.Order);

            await _workoutProgramRepository.UpdateAsync(program, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}