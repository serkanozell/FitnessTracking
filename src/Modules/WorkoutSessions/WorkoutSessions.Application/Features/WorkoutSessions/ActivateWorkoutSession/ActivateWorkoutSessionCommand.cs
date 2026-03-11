namespace WorkoutSessions.Application.Features.WorkoutSessions.ActivateWorkoutSession
{
    public sealed record ActivateWorkoutSessionCommand(Guid WorkoutSessionId) : ICommand<Result<Guid>>;
}