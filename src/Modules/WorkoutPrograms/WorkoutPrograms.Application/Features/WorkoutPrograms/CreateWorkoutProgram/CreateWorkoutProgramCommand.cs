using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    public sealed record CreateWorkoutProgramCommand(string Name,
                                                     DateTime StartDate,
                                                     DateTime EndDate) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [];
        public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
    }
}