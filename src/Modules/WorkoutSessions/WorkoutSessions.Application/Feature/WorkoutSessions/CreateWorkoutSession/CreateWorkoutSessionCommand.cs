namespace WorkoutSessions.Application.Feature.WorkoutSessions.CreateWorkoutSession
{
    public sealed record CreateWorkoutSessionCommand(Guid WorkoutProgramId,
                                                     DateTime Date) : ICommand<Result<Guid>>;
}