namespace WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession
{
    // NOTE: GetWorkoutSessionsQuery is user-scoped and therefore intentionally NOT
    // an ICacheableQuery (see docs/ARCHITECTURE.md / copilot-instructions.md).
    // There is no cache entry to invalidate, so we do not implement
    // ICacheInvalidatingCommand here. Re-introducing it would only trigger a
    // wasted Redis SCAN on every Create.
    //
    // Implements IIdempotentCommand so a duplicate POST carrying the same
    // X-Idempotency-Key header (e.g. a client retry) replays the original result
    // instead of creating a second session. The key is null when the header is
    // absent, which makes the IdempotencyBehavior a no-op.
    public sealed record CreateWorkoutSessionCommand(Guid WorkoutProgramId,
                                                     Guid WorkoutProgramSplitId,
                                                     DateTime Date,
                                                     string? IdempotencyKey = null)
        : ICommand<Result<Guid>>, IIdempotentCommand;
}