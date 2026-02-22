using BuildingBlocks.Domain.Abstractions;

namespace WorkoutSessions.Domain.Events
{
    public sealed record WorkoutSessionDeletedEvent(Guid SessionId, Guid ProgramId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
