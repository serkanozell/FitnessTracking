using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Pagination;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionsByProgram
{
    public sealed record GetWorkoutSessionsByProgramQuery(Guid WorkoutProgramId, int PageNumber = PaginationDefaults.DefaultPageNumber, int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<WorkoutSessionDto>>>, ICacheableQuery
    {
        public string CacheKey => $"workoutsessions:program:{WorkoutProgramId}:p{PageNumber}:s{PageSize}";
        public TimeSpan? Expiration => null;
    }
}