using BuildingBlocks.Domain.Abstractions;

namespace BuildingBlocks.Domain.Events
{
    public interface IDomainEventDispatcher
    {
        IReadOnlyList<IDomainEvent> CollectDomainEvents(IEnumerable<IAggregate> aggregates);
        Task DispatchDomainEvents(IEnumerable<IDomainEvent> domainEvents);
    }
}