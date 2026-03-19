using BuildingBlocks.Application.Abstractions.Caching;

namespace Exercises.Application.Features.Exercises.DeleteExercise
{
    public sealed record DeleteExerciseCommand(Guid Id) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"exercises:{Id}"];
        public string[] CachePrefixesToInvalidate => ["exercises:all"];
    }
}