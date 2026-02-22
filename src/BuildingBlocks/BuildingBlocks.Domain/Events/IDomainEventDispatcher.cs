using BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Domain.Events
{
    public interface IDomainEventDispatcher
    {
        IReadOnlyList<IDomainEvent> CollectDomainEvents(DbContext context);
        Task DispatchDomainEvents(IEnumerable<IDomainEvent> domainEvents);
    }
}