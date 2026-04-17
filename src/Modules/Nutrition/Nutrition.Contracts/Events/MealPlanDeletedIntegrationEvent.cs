using BuildingBlocks.Domain.Abstractions;

namespace Nutrition.Contracts.Events;

public sealed record MealPlanDeletedIntegrationEvent(Guid MealPlanId, string PerformedBy) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.Now;
}
