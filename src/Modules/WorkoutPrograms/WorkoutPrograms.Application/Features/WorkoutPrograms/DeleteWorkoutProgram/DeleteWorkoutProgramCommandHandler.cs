using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    internal sealed class DeleteWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<DeleteWorkoutProgramCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.Id);

            program.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}