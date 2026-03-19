using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram;

public sealed record ActivateWorkoutProgramCommand(Guid Id) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
{
    public string[] CacheKeysToInvalidate => [$"workoutprograms:{Id}"];
    public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
}