using BuildingBlocks.Application.Pagination;
using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions
{
    internal sealed class GetWorkoutSessionsQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetWorkoutSessionsQuery, Result<PagedResult<WorkoutSessionDto>>>
    {
        public async Task<Result<PagedResult<WorkoutSessionDto>>> Handle(GetWorkoutSessionsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = request.ProgramId.HasValue
                ? await _workoutSessionRepository.GetPagedByProgramAsync(request.ProgramId.Value, request.PageNumber, request.PageSize, cancellationToken)
                : await _workoutSessionRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            var dtos = items.Select(WorkoutSessionDto.FromEntity).ToList();

            return PagedResult<WorkoutSessionDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}