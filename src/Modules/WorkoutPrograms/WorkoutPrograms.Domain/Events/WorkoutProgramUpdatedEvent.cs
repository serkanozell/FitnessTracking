using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Domain.Events
{
    public sealed record WorkoutProgramUpdatedEvent(Guid ProgramId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
