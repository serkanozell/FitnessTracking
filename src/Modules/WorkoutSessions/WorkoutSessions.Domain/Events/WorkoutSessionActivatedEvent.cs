using BuildingBlocks.Domain.Abstractions;

namespace WorkoutSessions.Domain.Events
{
    public sealed record WorkoutSessionActivatedEvent(Guid SessionId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
