using BuildingBlocks.Domain.Abstractions;

namespace Exercises.Domain.Events
{
    public sealed record ExerciseCreatedEvent(Guid ExerciseId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
