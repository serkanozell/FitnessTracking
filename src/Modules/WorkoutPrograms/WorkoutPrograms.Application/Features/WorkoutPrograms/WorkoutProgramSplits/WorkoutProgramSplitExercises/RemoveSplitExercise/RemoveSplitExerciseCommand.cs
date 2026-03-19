using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    public sealed record RemoveSplitExerciseCommand(Guid WorkoutProgramId,
                                                    Guid WorkoutProgramSplitId,
                                                    Guid WorkoutProgramExerciseId) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutprograms:{WorkoutProgramId}", $"workoutprograms:{WorkoutProgramId}:splits:{WorkoutProgramSplitId}:exercises"];
        public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
    }
}