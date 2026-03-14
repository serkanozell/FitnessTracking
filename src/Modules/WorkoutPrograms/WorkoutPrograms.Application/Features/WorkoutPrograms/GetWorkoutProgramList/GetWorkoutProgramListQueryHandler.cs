using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    internal sealed class GetWorkoutProgramListQueryHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        ICurrentUser _currentUser) : IQueryHandler<GetWorkoutProgramListQuery, Result<PagedResult<WorkoutProgramDto>>>
    {
        public async Task<Result<PagedResult<WorkoutProgramDto>>> Handle(GetWorkoutProgramListQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var (items, totalCount) = await _workoutProgramRepository.GetPagedByUserAsync(userId, request.PageNumber, request.PageSize, cancellationToken);

            var dtos = items.Select(WorkoutProgramDto.FromEntity).ToList();

            return PagedResult<WorkoutProgramDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}