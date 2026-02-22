using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    internal sealed class AddWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<AddWorkoutProgramSplitCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            if (workoutProgram.Splits.Any(s => s.Name == request.Name))
                return WorkoutProgramErrors.SplitDuplicateName(request.Name);

            var split = workoutProgram.AddSplit(request.Name, request.Order);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return split.Id;
        }
    }
}