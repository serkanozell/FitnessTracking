namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession
{
    public sealed record RemoveExerciseFromSessionCommand(Guid WorkoutSessionId,
                                                          Guid SessionExerciseId) : ICommand<Result<bool>>;
}