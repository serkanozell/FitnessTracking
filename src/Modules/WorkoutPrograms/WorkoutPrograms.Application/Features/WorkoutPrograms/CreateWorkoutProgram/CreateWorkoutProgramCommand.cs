namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    public sealed record CreateWorkoutProgramCommand(string Name,
                                                     DateTime StartDate,
                                                     DateTime EndDate) : ICommand<Result<Guid>>;
}