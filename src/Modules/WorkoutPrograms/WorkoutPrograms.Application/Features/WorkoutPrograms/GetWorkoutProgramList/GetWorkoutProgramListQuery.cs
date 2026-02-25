using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Pagination;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    public sealed record GetWorkoutProgramListQuery(int PageNumber = PaginationDefaults.DefaultPageNumber, int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<WorkoutProgramDto>>>, ICacheableQuery
    {
        public string CacheKey => $"workoutprograms:all:p{PageNumber}:s{PageSize}";
        public TimeSpan? Expiration => null;
    }
}