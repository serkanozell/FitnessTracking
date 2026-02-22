using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Domain.Events
{
    public sealed record WorkoutProgramSplitChangedEvent(Guid ProgramId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}