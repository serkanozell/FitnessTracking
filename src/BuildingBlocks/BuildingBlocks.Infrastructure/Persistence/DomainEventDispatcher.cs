using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Events;
using MediatR;

namespace BuildingBlocks.Infrastructure.Persistence
{
    public sealed class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
    {
        public IReadOnlyList<IDomainEvent> CollectDomainEvents(IEnumerable<IAggregate> aggregates)
        {
            var aggregatesWithEvents = aggregates.Where(a => a.DomainEvents.Any()).ToList();

            var domainEvents = aggregatesWithEvents.SelectMany(a => a.DomainEvents).ToList();

            aggregatesWithEvents.ForEach(a => a.ClearDomainEvents());

            return domainEvents;
        }

        public async Task DispatchDomainEvents(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}