using BuildingBlocks.Domain.Abstractions;

namespace WorkoutSessions.Domain.Events
{
    public sealed record SessionExerciseChangedEvent(Guid SessionId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}