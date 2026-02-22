using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Domain.Events
{
    public sealed record WorkoutProgramActivatedEvent(Guid ProgramId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
