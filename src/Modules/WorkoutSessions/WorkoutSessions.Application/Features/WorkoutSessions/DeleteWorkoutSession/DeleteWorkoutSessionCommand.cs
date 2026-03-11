namespace WorkoutSessions.Application.Features.WorkoutSessions.DeleteWorkoutSession
{
    public sealed record DeleteWorkoutSessionCommand(Guid Id) : ICommand<Result<bool>>;
}