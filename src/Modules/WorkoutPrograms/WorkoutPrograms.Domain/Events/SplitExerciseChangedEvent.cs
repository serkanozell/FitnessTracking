using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Domain.Events
{
    public sealed record SplitExerciseChangedEvent(Guid ProgramId, Guid SplitId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}