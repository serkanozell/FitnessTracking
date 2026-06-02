namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    public sealed record AddExerciseToSessionCommand(Guid WorkoutSessionId,
                                                     Guid ExerciseId,
                                                     int SetNumber,
                                                     decimal Weight,
                                                     int Reps,
                                                     string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}