using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence
{
    public sealed class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
    {
        public IReadOnlyList<IDomainEvent> CollectDomainEvents(DbContext context)
        {
            var aggregates = context.ChangeTracker.Entries<IAggregate>()
                                                  .Where(a => a.Entity.DomainEvents.Any())
                                                  .Select(a => a.Entity)
                                                  .ToList();

            var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();

            aggregates.ForEach(a => a.ClearDomainEvents());

            return domainEvents;
        }

        public async Task DispatchDomainEvents(IEnumerable<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}