namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    public sealed class AddExerciseToSessionCommand : ICommand<Guid>
    {
        public Guid WorkoutSessionId { get; init; }
        public Guid WorkoutSplitId { get; init; }
        public Guid ExerciseId { get; init; }
        public int SetNumber { get; init; }
        public decimal Weight { get; init; }
        public int Reps { get; init; }
    }
}