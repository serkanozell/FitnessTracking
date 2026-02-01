namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.UpdateExerciseInSession
{
    public sealed class UpdateExerciseInSessionCommand : ICommand<Unit>
    {
        public Guid WorkoutSessionId { get; init; }
        public Guid WorkoutExerciseId { get; init; }
        public int SetNumber { get; init; }
        public decimal Weight { get; init; }
        public int Reps { get; init; }
    }
}