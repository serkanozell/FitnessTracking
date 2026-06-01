namespace WorkoutSessions.Application.Features.WorkoutSessions.UpdateWorkoutSession
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record UpdateWorkoutSessionCommand(Guid Id,
                                                     DateTime Date) : ICommand<Result<bool>>;
}