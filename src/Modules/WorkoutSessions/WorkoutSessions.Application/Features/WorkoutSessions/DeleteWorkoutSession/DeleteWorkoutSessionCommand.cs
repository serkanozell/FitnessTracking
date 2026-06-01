namespace WorkoutSessions.Application.Features.WorkoutSessions.DeleteWorkoutSession
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record DeleteWorkoutSessionCommand(Guid Id) : ICommand<Result<bool>>;
}