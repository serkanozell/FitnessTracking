using BuildingBlocks.Application.Abstractions.Caching;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    public sealed record CreateExerciseCommand(string Name, string PrimaryMuscleGroup, string? SecondaryMuscleGroup, string Description) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [];
        public string[] CachePrefixesToInvalidate => ["exercises:all"];
    }
}