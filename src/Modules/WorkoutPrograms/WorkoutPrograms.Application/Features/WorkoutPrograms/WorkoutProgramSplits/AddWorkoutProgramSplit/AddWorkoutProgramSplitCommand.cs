namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    public sealed class AddWorkoutProgramSplitCommand : ICommand<Guid>
    {
        public Guid WorkoutProgramId { get; }
        public string Name { get; }
        public int Order { get; }

        public AddWorkoutProgramSplitCommand(Guid workoutProgramId, string name, int order)
        {
            WorkoutProgramId = workoutProgramId;
            Name = name;
            Order = order;
        }
    }
}