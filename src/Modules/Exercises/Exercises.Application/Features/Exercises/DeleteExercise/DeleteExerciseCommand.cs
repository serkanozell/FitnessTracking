using BuildingBlocks.Application.Abstractions.Caching;
using Exercises.Application.Caching;

namespace Exercises.Application.Features.Exercises.DeleteExercise
{
    public sealed record DeleteExerciseCommand(Guid Id) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [ExerciseCacheKeys.ById(Id)];
        public string[] CachePrefixesToInvalidate => [ExerciseCacheKeys.AllPrefix];
    }
}