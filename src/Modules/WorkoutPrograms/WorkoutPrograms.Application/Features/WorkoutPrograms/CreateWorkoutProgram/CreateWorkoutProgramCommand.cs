namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record CreateWorkoutProgramCommand(string Name,
                                                     string? Description,
                                                     DateTime StartDate,
                                                     DateTime EndDate,
                                                     string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}