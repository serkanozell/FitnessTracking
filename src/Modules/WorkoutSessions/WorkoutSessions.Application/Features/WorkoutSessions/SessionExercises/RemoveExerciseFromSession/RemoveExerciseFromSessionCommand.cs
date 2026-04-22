namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.RemoveExerciseFromSession
{
    public sealed record RemoveExerciseFromSessionCommand(Guid WorkoutSessionId,
                                                          Guid SessionExerciseId) : ICommand<Result<bool>>;
}