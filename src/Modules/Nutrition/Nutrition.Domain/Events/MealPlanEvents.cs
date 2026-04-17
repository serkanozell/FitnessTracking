using BuildingBlocks.Domain.Abstractions;

namespace Nutrition.Domain.Events
{
    public sealed record MealPlanCreatedEvent(Guid MealPlanId, Guid UserId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record MealPlanUpdatedEvent(Guid MealPlanId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record MealPlanActivatedEvent(Guid MealPlanId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record MealPlanDeletedEvent(Guid MealPlanId, string DeletedBy) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record MealPlanMealChangedEvent(Guid MealPlanId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record MealItemChangedEvent(Guid MealPlanId, Guid MealId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
