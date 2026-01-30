namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    public sealed class DeleteWorkoutProgramSplitCommand : ICommand<bool>
    {
        public Guid WorkoutProgramId { get; init; }
        public Guid SplitId { get; init; }
    }
}