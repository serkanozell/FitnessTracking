using BuildingBlocks.Application.Abstractions.Caching;

namespace Exercises.Application.Features.Exercises.UpdateExercise
{
    public sealed record UpdateExerciseCommand(Guid Id,
                                               string Name,
                                               string PrimaryMuscleGroup,
                                               string? SecondaryMuscleGroup,
                                               string Description,
                                               string? ImageUrl,
                                               string? VideoUrl) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"exercises:{Id}"];
        public string[] CachePrefixesToInvalidate => ["exercises:all"];
    }
}