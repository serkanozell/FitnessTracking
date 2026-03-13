using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxDbContext(DbContextOptions<OutboxDbContext> options)
        : ModuleDbContext(options)
    {
    }
}