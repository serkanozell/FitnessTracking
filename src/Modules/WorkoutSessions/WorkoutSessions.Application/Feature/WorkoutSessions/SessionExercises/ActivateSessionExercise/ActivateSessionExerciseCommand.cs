namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.ActivateSessionExercise
{
    public sealed record ActivateSessionExerciseCommand(Guid WorkoutSessionId,
                                                        Guid SessionExerciseId) : ICommand<Result<Guid>>;
}