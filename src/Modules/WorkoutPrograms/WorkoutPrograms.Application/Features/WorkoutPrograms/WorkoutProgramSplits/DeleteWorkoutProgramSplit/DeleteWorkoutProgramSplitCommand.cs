namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    public sealed record DeleteWorkoutProgramSplitCommand(Guid WorkoutProgramId, Guid SplitId) : ICommand<Result<bool>>;
}