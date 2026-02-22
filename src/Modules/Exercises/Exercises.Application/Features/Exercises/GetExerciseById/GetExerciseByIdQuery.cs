using BuildingBlocks.Application.Abstractions.Caching;
using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetExerciseById
{
    public sealed record GetExerciseByIdQuery(Guid Id) : IQuery<Result<ExerciseDto>>, ICacheableQuery
    {
        public string CacheKey => $"exercises:{Id}";
        public TimeSpan? Expiration => null;
    }
}