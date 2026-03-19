using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    public sealed record AddWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                       string Name,
                                                       int Order) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutprograms:{WorkoutProgramId}", $"workoutprograms:{WorkoutProgramId}:splits"];
        public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
    }
}