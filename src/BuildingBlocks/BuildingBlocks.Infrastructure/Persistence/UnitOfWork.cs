using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence
{
    public abstract class UnitOfWork<TContext>(TContext context, IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
        where TContext : DbContext
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await domainEventDispatcher.DispatchDomainEvents(context);

            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}