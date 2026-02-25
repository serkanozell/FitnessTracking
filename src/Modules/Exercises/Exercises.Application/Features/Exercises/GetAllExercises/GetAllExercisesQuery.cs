using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Pagination;
using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    public sealed record GetAllExercisesQuery(int PageNumber = PaginationDefaults.DefaultPageNumber, int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<ExerciseDto>>>, ICacheableQuery
    {
        public string CacheKey => $"exercises:all:p{PageNumber}:s{PageSize}";
        public TimeSpan? Expiration => null;
    }
}