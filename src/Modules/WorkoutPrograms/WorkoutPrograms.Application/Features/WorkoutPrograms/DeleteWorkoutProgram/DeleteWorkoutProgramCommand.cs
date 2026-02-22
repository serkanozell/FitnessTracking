namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    public sealed record DeleteWorkoutProgramCommand(Guid Id) : ICommand<Result<bool>>;
}