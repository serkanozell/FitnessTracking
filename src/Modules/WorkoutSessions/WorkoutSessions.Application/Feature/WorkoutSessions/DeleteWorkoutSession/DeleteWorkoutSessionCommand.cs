namespace WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession
{
    public sealed record DeleteWorkoutSessionCommand(Guid Id) : ICommand<Result<bool>>;
}