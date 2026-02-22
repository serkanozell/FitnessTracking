using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Domain.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchDomainEvents(DbContext context);
    }
}