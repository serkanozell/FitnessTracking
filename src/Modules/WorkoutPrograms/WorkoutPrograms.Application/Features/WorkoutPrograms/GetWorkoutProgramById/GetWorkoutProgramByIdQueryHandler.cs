using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById
{
    internal sealed class GetWorkoutProgramByIdQueryHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        ICurrentUser _currentUser) : IQueryHandler<GetWorkoutProgramByIdQuery, Result<WorkoutProgramDto>>
    {
        public async Task<Result<WorkoutProgramDto>> Handle(GetWorkoutProgramByIdQuery request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, program.UserId);
            if (ownershipError is not null)
                return ownershipError;

            return WorkoutProgramDto.FromEntity(program);
        }
    }
}