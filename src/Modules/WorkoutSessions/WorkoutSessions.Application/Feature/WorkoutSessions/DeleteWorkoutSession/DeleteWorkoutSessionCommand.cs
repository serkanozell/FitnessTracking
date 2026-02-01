namespace WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession
{
    public sealed class DeleteWorkoutSessionCommand : ICommand<Unit>
    {
        public Guid Id { get; init; }
    }
}