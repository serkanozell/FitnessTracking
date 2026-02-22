namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram;

public sealed record ActivateWorkoutProgramCommand(Guid Id) : ICommand<Result<Guid>>;