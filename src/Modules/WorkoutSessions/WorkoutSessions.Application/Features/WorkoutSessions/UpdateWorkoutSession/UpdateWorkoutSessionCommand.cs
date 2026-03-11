namespace WorkoutSessions.Application.Features.WorkoutSessions.UpdateWorkoutSession
{
    public sealed record UpdateWorkoutSessionCommand(Guid Id,
                                                     DateTime Date) : ICommand<Result<bool>>;
}