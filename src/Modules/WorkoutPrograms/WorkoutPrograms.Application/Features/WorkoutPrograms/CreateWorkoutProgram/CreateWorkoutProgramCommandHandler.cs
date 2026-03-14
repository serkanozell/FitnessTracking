using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    internal sealed class CreateWorkoutProgramCommandHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IWorkoutProgramsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<CreateWorkoutProgramCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateWorkoutProgramCommand request, CancellationToken cancellationToken)
        {
            if (request.EndDate <= request.StartDate)
                return WorkoutProgramErrors.InvalidDateRange();

            var userId = Guid.Parse(_currentUser.UserId!);

            var workoutProgram = WorkoutProgram.Create(userId, request.Name, request.StartDate, request.EndDate);

            await _workoutProgramRepository.AddAsync(workoutProgram, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return workoutProgram.Id;
        }
    }
}