using BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence
{
    public abstract class UnitOfWork<TContext>(TContext context) : IUnitOfWork
        where TContext : DbContext
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}