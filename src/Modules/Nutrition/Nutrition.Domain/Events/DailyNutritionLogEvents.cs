using BuildingBlocks.Domain.Abstractions;

namespace Nutrition.Domain.Events
{
    public sealed record DailyNutritionLogCreatedEvent(Guid LogId, Guid UserId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record DailyNutritionLogUpdatedEvent(Guid LogId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record DailyNutritionLogDeletedEvent(Guid LogId, string DeletedBy) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record DailyNutritionLogEntryChangedEvent(Guid LogId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
