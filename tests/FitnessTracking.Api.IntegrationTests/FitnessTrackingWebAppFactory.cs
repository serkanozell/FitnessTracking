using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Infrastructure.Outbox;
using Exercises.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WorkoutPrograms.Infrastructure.Persistence;
using WorkoutSessions.Infrastructure.Persistence;

namespace FitnessTracking.Api.IntegrationTests;

public class FitnessTrackingWebAppFactory : WebApplicationFactory<Program>
{
    public static readonly Guid TestUserId = Guid.NewGuid();

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
            // Override authentication with a test scheme that always succeeds
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

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

            // Fake authenticated user for integration tests
            RemoveAllOfType<ICurrentUser>(services);
            var fakeUser = Substitute.For<ICurrentUser>();
            fakeUser.UserId.Returns(TestUserId.ToString());
            fakeUser.IsAuthenticated.Returns(true);
            fakeUser.IsAdmin.Returns(true);
            services.AddSingleton(fakeUser);

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

    private sealed class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, TestUserId.ToString()),
                new Claim(ClaimTypes.Name, "testuser@test.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
