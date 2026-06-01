namespace WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record UpdateWorkoutProgramCommand(Guid Id,
                                                     string Name,
                                                     string? Description,
                                                     DateTime StartDate,
                                                     DateTime EndDate) : ICommand<Result<bool>>;
}