using BuildingBlocks.Domain.Abstractions;

namespace WorkoutSessions.Domain.Events
{
    public sealed record WorkoutSessionUpdatedEvent(Guid SessionId, Guid ProgramId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}