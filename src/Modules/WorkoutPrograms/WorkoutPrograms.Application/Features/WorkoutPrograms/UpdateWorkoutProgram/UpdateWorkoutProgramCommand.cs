namespace WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram
{
    public sealed record UpdateWorkoutProgramCommand(Guid Id,
                                                     string Name,
                                                     DateTime StartDate,
                                                     DateTime EndDate) : ICommand<Result<bool>>;
}