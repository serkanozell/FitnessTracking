using BuildingBlocks.Application.Pagination;
using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionsByProgram
{
    internal sealed class GetWorkoutSessionsByProgramQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetWorkoutSessionsByProgramQuery, Result<PagedResult<WorkoutSessionDto>>>
    {
        public async Task<Result<PagedResult<WorkoutSessionDto>>> Handle(
            GetWorkoutSessionsByProgramQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _workoutSessionRepository.GetPagedByProgramAsync(
                request.WorkoutProgramId, request.PageNumber, request.PageSize, cancellationToken);

            var dtos = items.Select(WorkoutSessionDto.FromEntity).ToList();

            return PagedResult<WorkoutSessionDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}