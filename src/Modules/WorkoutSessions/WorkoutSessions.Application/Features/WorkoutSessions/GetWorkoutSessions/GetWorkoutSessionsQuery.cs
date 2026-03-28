using BuildingBlocks.Application.Pagination;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessions
{
    public sealed record GetWorkoutSessionsQuery(Guid? ProgramId = null, int PageNumber = PaginationDefaults.DefaultPageNumber, int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<WorkoutSessionDto>>>;
}