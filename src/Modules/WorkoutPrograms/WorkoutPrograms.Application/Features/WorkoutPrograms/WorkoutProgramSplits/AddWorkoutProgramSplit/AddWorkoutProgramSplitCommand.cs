namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    public sealed record AddWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                       string Name,
                                                       int Order) : ICommand<Result<Guid>>;
}