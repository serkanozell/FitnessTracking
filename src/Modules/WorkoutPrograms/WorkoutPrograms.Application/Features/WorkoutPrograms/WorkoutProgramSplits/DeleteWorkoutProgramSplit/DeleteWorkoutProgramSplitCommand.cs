namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record DeleteWorkoutProgramSplitCommand(Guid WorkoutProgramId, Guid SplitId) : ICommand<Result<bool>>;
}