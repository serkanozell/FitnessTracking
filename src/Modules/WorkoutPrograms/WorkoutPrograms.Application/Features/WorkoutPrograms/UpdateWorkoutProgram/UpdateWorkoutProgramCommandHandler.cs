using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram
{
    internal sealed class UpdateWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<UpdateWorkoutProgramCommand, Unit>
    {
        public async Task<Unit> Handle(UpdateWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);
            if (workoutProgram is null)
            {
                // Burada kendi NotFound exception tipini kullanabilirsin
                throw new KeyNotFoundException($"WorkoutProgram ({request.Id}) not found.");
            }

            workoutProgram.Update(request.Name, request.StartDate, request.EndDate);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}