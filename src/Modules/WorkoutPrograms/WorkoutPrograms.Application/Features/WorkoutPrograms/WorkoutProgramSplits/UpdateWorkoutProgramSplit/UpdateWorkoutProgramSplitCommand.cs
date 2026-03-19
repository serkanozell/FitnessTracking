using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    public sealed record UpdateWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                          Guid SplitId,
                                                          string Name,
                                                          int Order) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutprograms:{WorkoutProgramId}", $"workoutprograms:{WorkoutProgramId}:splits"];
        public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
    }
}