using BuildingBlocks.Domain.Abstractions;

namespace Nutrition.Domain.Events
{
    public sealed record FoodCreatedEvent(Guid FoodId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record FoodUpdatedEvent(Guid FoodId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record FoodActivatedEvent(Guid FoodId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record FoodDeletedEvent(Guid FoodId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
