namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    public sealed record UpdateWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                          Guid SplitId,
                                                          string Name,
                                                          int Order) : ICommand<Result<bool>>;
}