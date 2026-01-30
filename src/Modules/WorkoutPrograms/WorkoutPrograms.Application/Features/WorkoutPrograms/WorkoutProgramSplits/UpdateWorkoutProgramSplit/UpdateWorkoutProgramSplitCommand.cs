namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    public sealed class UpdateWorkoutProgramSplitCommand : ICommand<bool>
    {
        public Guid WorkoutProgramId { get; init; }
        public Guid SplitId { get; init; }
        public string Name { get; init; } = default!;
        public int Order { get; init; }
    }
}