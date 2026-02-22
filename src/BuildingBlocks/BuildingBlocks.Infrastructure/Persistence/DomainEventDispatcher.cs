using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence
{
    public sealed class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
    {
        public async Task DispatchDomainEvents(DbContext? context)
        {
            if (context is null) return;

            var aggregates = context.ChangeTracker.Entries<IAggregate>()
                                                  .Where(a => a.Entity.DomainEvents.Any())
                                                  .Select(a => a.Entity);

            var domainEvents = aggregates.SelectMany(a => a.DomainEvents)
                                         .ToList();

            aggregates.ToList().ForEach(a => a.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}