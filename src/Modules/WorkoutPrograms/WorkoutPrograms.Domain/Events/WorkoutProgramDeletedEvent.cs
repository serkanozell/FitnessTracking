using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Domain.Events
{
    public sealed record WorkoutProgramDeletedEvent(Guid ProgramId, string DeletedBy) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}