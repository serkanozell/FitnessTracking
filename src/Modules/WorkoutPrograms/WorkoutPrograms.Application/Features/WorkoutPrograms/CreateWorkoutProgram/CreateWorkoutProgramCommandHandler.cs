using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    internal sealed class CreateWorkoutProgramCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<CreateWorkoutProgramCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            if (request.EndDate <= request.StartDate)
                return WorkoutProgramErrors.InvalidDateRange();

            var workoutProgram = WorkoutProgram.Create(request.Name, request.StartDate, request.EndDate);

            await _workoutProgramRepository.AddAsync(workoutProgram, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return workoutProgram.Id;
        }
    }
}