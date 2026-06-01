using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Pagination;
using Exercises.Application.Caching;
using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    public sealed record GetAllExercisesQuery(int PageNumber = PaginationDefaults.DefaultPageNumber, int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<ExerciseDto>>>, ICacheableQuery
    {
        public string CacheKey => ExerciseCacheKeys.All(PageNumber, PageSize);
        public TimeSpan? Expiration => null;
    }
}