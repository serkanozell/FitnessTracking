using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits
{
    internal sealed class GetWorkoutProgramSplitsQueryHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        ICurrentUser _currentUser) : IQueryHandler<GetWorkoutProgramSplitsQuery, Result<IReadOnlyList<WorkoutProgramSplitDto>>>
    {
        public async Task<Result<IReadOnlyList<WorkoutProgramSplitDto>>> Handle(GetWorkoutProgramSplitsQuery request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, program.UserId);
            if (ownershipError is not null)
                return ownershipError;

            return program.Splits.Select(WorkoutProgramSplitDto.FromEntity)
                                 .OrderBy(x => x.Order)
                                 .ToList();
        }
    }
}