using BuildingBlocks.Domain.Abstractions;

namespace Exercises.Domain.Events
{
    public sealed record ExerciseActivatedEvent(Guid ExerciseId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
