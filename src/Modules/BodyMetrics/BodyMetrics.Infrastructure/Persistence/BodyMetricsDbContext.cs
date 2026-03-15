using BuildingBlocks.Infrastructure.Persistence;
using BodyMetrics.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace BodyMetrics.Infrastructure.Persistence
{
    public sealed class BodyMetricsDbContext(DbContextOptions<BodyMetricsDbContext> options)
        : ModuleDbContext(options)
    {
        public DbSet<BodyMetric> BodyMetrics => Set<BodyMetric>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BodyMetricsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}