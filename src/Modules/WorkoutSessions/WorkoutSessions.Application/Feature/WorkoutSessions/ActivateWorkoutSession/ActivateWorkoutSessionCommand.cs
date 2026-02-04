namespace WorkoutSessions.Application.Feature.WorkoutSessions.ActivateWorkoutSession
{
    public sealed record ActivateWorkoutSessionCommand(Guid WorkoutSessionId) : ICommand<Guid>;
}