using BuildingBlocks.Application.Abstractions.Caching;

namespace Exercises.Application.Features.Exercises.ActivateExercise
{
    public sealed record ActivateExerciseCommand(Guid Id) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"exercises:{Id}"];
        public string[] CachePrefixesToInvalidate => ["exercises:all"];
    }
}