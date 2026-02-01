namespace WorkoutSessions.Application.Feature.WorkoutSessions.CreateWorkoutSession
{
    public sealed class CreateWorkoutSessionCommand : ICommand<Guid>
    {
        public Guid WorkoutProgramId { get; init; }
        public DateTime Date { get; init; }
    }
}