using BuildingBlocks.Application.Abstractions.Caching;
using Exercises.Application.Caching;

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
        public string[] CacheKeysToInvalidate => [ExerciseCacheKeys.ById(Id)];
        public string[] CachePrefixesToInvalidate => [ExerciseCacheKeys.AllPrefix];
    }
}