namespace WorkoutSessions.Application.Feature.WorkoutSessions.UpdateWorkoutSession
{
    public sealed class UpdateWorkoutSessionCommand : ICommand<Unit>
    {
        public Guid Id { get; init; }
        public DateTime Date { get; init; }
    }
}