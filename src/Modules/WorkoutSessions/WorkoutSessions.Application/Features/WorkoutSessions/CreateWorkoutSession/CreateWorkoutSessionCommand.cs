namespace WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession
{
    public sealed record CreateWorkoutSessionCommand(Guid WorkoutProgramId,
                                                     DateTime Date) : ICommand<Result<Guid>>;
}