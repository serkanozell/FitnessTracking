using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Infrastructure.Outbox;
using Exercises.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using StackExchange.Redis;
using WorkoutPrograms.Infrastructure.Persistence;
using WorkoutSessions.Infrastructure.Persistence;

namespace FitnessTracking.Api.IntegrationTests;

public class FitnessTrackingWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("DetailedErrors", "true");

        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:FitnessDbConnection"] = "DataSource=:memory:",
                ["ConnectionStrings:Redis"] = "localhost:9999",
                ["Redis:ConnectionString"] = "localhost:9999"
            });
        });

        builder.ConfigureServices(services =>
        {
            ReplaceDbContext<ExercisesDbContext>(services);
            ReplaceDbContext<WorkoutProgramsDbContext>(services);
            ReplaceDbContext<WorkoutSessionsDbContext>(services);
            ReplaceDbContext<OutboxDbContext>(services);

            RemoveAllOfType<IConnectionMultiplexer>(services);
            services.AddSingleton(Substitute.For<IConnectionMultiplexer>());

            RemoveAllOfType<ICacheService>(services);
            RemoveAllOfType<ICacheAsideService>(services);
            services.AddSingleton(Substitute.For<ICacheService>());

            // CacheAsideService pass-through: just call the factory delegate (no caching)
            services.AddSingleton<ICacheAsideService>(new PassThroughCacheAsideService());

            RemoveAllOfType<IHostedService>(services);

            var toRemove = services
                .Where(d => (d.ServiceType.FullName ?? "").Contains("HealthCheck")
                         || (d.ImplementationType?.FullName ?? "").Contains("HealthCheck"))
                .ToList();
            foreach (var d in toRemove)
                services.Remove(d);
            services.AddHealthChecks();
        });
    }

    private static void ReplaceDbContext<TContext>(IServiceCollection services) where TContext : DbContext
    {
        // Remove ALL registrations related to this DbContext type,
        // including IDbContextOptionsConfiguration<TContext> which accumulates provider configurations.
        var contextTypeName = typeof(TContext).FullName!;
        var descriptors = services
            .Where(d => d.ServiceType == typeof(DbContextOptions<TContext>)
                     || d.ServiceType == typeof(TContext)
                     || (d.ServiceType.IsGenericType
                         && d.ServiceType.GenericTypeArguments.Contains(typeof(TContext)))
                     || (d.ImplementationType?.FullName?.Contains(contextTypeName) ?? false))
            .ToList();
        foreach (var d in descriptors)
            services.Remove(d);

        var dbName = "Test_" + typeof(TContext).Name + "_" + Guid.NewGuid().ToString("N");
        services.AddDbContext<TContext>(o => o.UseInMemoryDatabase(dbName));
    }

    private static void RemoveAllOfType<T>(IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(T)).ToList();
        foreach (var d in descriptors)
            services.Remove(d);
    }

    private sealed class PassThroughCacheAsideService : ICacheAsideService
    {
        public Task<T> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            return factory(cancellationToken);
        }
    }
}
