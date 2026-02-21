namespace WorkoutSessions.Application.Feature.WorkoutSessions.UpdateWorkoutSession
{
    public sealed record UpdateWorkoutSessionCommand(Guid Id,
                                                     DateTime Date) : ICommand<Result<bool>>;
}