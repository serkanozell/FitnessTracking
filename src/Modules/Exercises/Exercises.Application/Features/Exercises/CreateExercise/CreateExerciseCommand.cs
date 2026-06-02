using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Abstractions.Idempotency;
using Exercises.Application.Caching;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    public sealed record CreateExerciseCommand(string Name, string PrimaryMuscleGroup, string? SecondaryMuscleGroup, string Description, string? ImageUrl, string? VideoUrl, string? IdempotencyKey = null) : ICommand<Result<Guid>>, ICacheInvalidatingCommand, IIdempotentCommand
    {
        public string[] CacheKeysToInvalidate => [];
        public string[] CachePrefixesToInvalidate => [ExerciseCacheKeys.AllPrefix];
    }
}