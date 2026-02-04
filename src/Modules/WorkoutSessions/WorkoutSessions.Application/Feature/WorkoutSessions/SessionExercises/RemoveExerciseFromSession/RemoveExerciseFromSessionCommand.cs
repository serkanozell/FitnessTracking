namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession
{
    public sealed class RemoveExerciseFromSessionCommand : ICommand<Unit>
    {
        public Guid WorkoutSessionId { get; init; }
        public Guid SessionExerciseId { get; init; }
    }
}