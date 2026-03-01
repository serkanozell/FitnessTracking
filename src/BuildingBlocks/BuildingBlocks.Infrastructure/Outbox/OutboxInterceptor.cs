using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                ConvertDomainEventsToOutboxMessages(eventData.Context);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
            {
                ConvertDomainEventsToOutboxMessages(eventData.Context);
            }

            return base.SavingChanges(eventData, result);
        }

        private static void ConvertDomainEventsToOutboxMessages(DbContext context)
        {
            var aggregates = context.ChangeTracker.Entries<IAggregate>()
                                                  .Select(e => e.Entity)
                                                  .Where(e => e.DomainEvents.Any())
                                                  .ToList();

            if (aggregates.Count == 0)
                return;

            var domainEvents = aggregates.SelectMany(a => a.DomainEvents)
                                         .ToList();

            aggregates.ForEach(a => a.ClearDomainEvents());

            var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                EventType = domainEvent.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                IsProcessed = false,
                OccurredOnUtc = DateTime.UtcNow,
                ProcessedOnUtc = null,
                Error = null
            }).ToList();

            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }
    }
}
