using BuildingBlocks.Application.Abstractions.Caching;
using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    public sealed record GetAllExercisesQuery : IQuery<Result<IReadOnlyList<ExerciseDto>>>, ICacheableQuery
    {
        public string CacheKey => "exercises:all";
        public TimeSpan? Expiration => null;
    }
}