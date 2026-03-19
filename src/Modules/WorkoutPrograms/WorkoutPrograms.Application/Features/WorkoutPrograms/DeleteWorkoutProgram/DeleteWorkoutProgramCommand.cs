using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    public sealed record DeleteWorkoutProgramCommand(Guid Id) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutprograms:{Id}"];
        public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
    }
}