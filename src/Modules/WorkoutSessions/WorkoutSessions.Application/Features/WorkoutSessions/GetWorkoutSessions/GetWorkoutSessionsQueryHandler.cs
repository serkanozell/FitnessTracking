using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessions
{
    internal sealed class GetWorkoutSessionsQueryHandler(
        IWorkoutSessionRepository _workoutSessionRepository,
        ICurrentUser _currentUser) : IQueryHandler<GetWorkoutSessionsQuery, Result<PagedResult<WorkoutSessionDto>>>
    {
        public async Task<Result<PagedResult<WorkoutSessionDto>>> Handle(GetWorkoutSessionsQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var (items, totalCount) = request.ProgramId.HasValue
                ? await _workoutSessionRepository.GetPagedByUserAndProgramAsync(userId, request.ProgramId.Value, request.PageNumber, request.PageSize, cancellationToken)
                : await _workoutSessionRepository.GetPagedByUserAsync(userId, request.PageNumber, request.PageSize, cancellationToken);

            var dtos = items.Select(WorkoutSessionDto.FromEntity).ToList();

            return PagedResult<WorkoutSessionDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}