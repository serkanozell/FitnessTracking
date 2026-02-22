using BuildingBlocks.Domain.Abstractions;

namespace Exercises.Domain.Events
{
    public sealed record ExerciseDeletedEvent(Guid ExerciseId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}