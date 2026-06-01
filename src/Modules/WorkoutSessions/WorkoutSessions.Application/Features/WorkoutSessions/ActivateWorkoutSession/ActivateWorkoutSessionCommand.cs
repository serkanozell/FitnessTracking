namespace WorkoutSessions.Application.Features.WorkoutSessions.ActivateWorkoutSession
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record ActivateWorkoutSessionCommand(Guid WorkoutSessionId) : ICommand<Result<Guid>>;
}