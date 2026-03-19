using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;

public sealed record ActivateSplitExerciseCommand(Guid WorkoutProgramId,
                                                  Guid SplitId,
                                                  Guid WorkoutSplitExerciseId) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
{
    public string[] CacheKeysToInvalidate => [$"workoutprograms:{WorkoutProgramId}", $"workoutprograms:{WorkoutProgramId}:splits:{SplitId}:exercises"];
    public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
}