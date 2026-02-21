using BuildingBlocks.Application.Results;
using WorkoutPrograms.Application.Errors;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram
{
    internal sealed class UpdateWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<UpdateWorkoutProgramCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (workoutProgram is null)
                return WorkoutProgramErrors.NotFound(request.Id);

            if (request.EndDate <= request.StartDate)
                return WorkoutProgramErrors.InvalidDateRange();

            workoutProgram.Update(request.Name, request.StartDate, request.EndDate);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}