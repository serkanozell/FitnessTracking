using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;

public sealed record ActivateWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                        Guid SplitId) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
{
    public string[] CacheKeysToInvalidate => [$"workoutprograms:{WorkoutProgramId}", $"workoutprograms:{WorkoutProgramId}:splits"];
    public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
}