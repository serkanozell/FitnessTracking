namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram;

// User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
// nothing to invalidate here.
public sealed record ActivateWorkoutProgramCommand(Guid Id) : ICommand<Result<Guid>>;