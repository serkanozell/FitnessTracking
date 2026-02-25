using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Pagination;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions
{
    public sealed record GetWorkoutSessionsQuery(int PageNumber = PaginationDefaults.DefaultPageNumber, int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<WorkoutSessionDto>>>, ICacheableQuery
    {
        public string CacheKey => $"workoutsessions:all:p{PageNumber}:s{PageSize}";
        public TimeSpan? Expiration => null;
    }
}