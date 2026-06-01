using BuildingBlocks.Application.Abstractions.Caching;
using Exercises.Application.Caching;

namespace Exercises.Application.Features.Exercises.ActivateExercise
{
    public sealed record ActivateExerciseCommand(Guid Id) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [ExerciseCacheKeys.ById(Id)];
        public string[] CachePrefixesToInvalidate => [ExerciseCacheKeys.AllPrefix];
    }
}