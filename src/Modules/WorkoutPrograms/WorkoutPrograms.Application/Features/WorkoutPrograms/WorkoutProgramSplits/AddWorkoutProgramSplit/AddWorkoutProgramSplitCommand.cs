namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record AddWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                       string Name,
                                                       int Order) : ICommand<Result<Guid>>;
}