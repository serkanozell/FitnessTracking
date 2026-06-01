namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record UpdateWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                          Guid SplitId,
                                                          string Name,
                                                          int Order) : ICommand<Result<bool>>;
}