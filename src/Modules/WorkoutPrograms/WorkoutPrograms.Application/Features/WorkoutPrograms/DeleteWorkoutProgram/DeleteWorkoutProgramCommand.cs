namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record DeleteWorkoutProgramCommand(Guid Id) : ICommand<Result<bool>>;
}