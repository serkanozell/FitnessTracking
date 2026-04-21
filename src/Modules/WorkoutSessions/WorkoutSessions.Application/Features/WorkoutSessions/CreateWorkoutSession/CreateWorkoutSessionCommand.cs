namespace WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession
{
    // NOTE: GetWorkoutSessionsQuery is user-scoped and therefore intentionally NOT
    // an ICacheableQuery (see docs/ARCHITECTURE.md / copilot-instructions.md).
    // There is no cache entry to invalidate, so we do not implement
    // ICacheInvalidatingCommand here. Re-introducing it would only trigger a
    // wasted Redis SCAN on every Create.
    public sealed record CreateWorkoutSessionCommand(Guid WorkoutProgramId,
                                                     Guid WorkoutProgramSplitId,
                                                     DateTime Date) : ICommand<Result<Guid>>;
}