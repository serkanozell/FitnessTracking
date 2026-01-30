using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    internal sealed class DeleteWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : IRequestHandler<DeleteWorkoutProgramCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteWorkoutProgramCommand request,
            CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (workoutProgram is null)
            {
                return false; // Idempotent behavior
            }

            await _workoutProgramRepository.DeleteAsync(request.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}