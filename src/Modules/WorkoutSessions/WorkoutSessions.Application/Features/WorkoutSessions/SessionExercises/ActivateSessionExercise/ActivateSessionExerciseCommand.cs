namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.ActivateSessionExercise
{
    public sealed record ActivateSessionExerciseCommand(Guid WorkoutSessionId,
                                                        Guid SessionExerciseId) : ICommand<Result<Guid>>;
}