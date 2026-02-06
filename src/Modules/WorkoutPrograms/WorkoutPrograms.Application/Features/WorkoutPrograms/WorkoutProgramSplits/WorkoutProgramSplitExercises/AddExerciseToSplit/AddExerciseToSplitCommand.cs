namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    public sealed class AddExerciseToSplitCommand : ICommand<Guid>
    {
        public Guid WorkoutProgramId { get; init; }
        public Guid WorkoutProgramSplitId { get; init; }
        public Guid ExerciseId { get; init; }
        public int Sets { get; init; }
        public int MinimumReps { get; init; }
        public int MaximumReps { get; init; }
    }
}