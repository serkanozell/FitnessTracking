using BuildingBlocks.Domain.Abstractions;

namespace WorkoutSessions.Domain.Events
{
    public sealed record WorkoutSessionCreatedEvent(Guid SessionId, Guid ProgramId, Guid SplitId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}