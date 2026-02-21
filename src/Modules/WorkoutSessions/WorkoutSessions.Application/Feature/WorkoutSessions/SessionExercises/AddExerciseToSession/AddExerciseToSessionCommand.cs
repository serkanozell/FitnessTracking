namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    public sealed record AddExerciseToSessionCommand(Guid WorkoutSessionId,
                                                     Guid ExerciseId,
                                                     int SetNumber,
                                                     decimal Weight,
                                                     int Reps) : ICommand<Result<Guid>>;
}