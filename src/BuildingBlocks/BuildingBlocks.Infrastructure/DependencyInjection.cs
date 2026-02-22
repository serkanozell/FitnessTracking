using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Email;
using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Persistence.Caching;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBuildingBlocksInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            AddEmail(services, configuration);
            AddRedisCaching(services, configuration);
            AddHealthChecks(services, configuration);

            return services;
        }

        public static IServiceCollection AddEmail(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<EmailOptions>(
                configuration.GetSection("EmailOptions"));

            services.AddScoped<IEmailSender, SmtpEmailSender>();

            return services;
        }

        public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.Configure<RedisOptions>(configuration.GetSection("Redis"));

            services.Configure<CacheOptions>(configuration.GetSection("Caching"));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis") ?? configuration["Redis:ConnectionString"];
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<ICacheAsideService, CacheAsideService>();

            return services;
        }

        private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
        {
            var redisConnection = configuration.GetConnectionString("Redis")
                                  ?? configuration["Redis:ConnectionString"]
                                  ?? "localhost:6379";

            var sqlConnection = configuration.GetConnectionString("FitnessDbConnection")
                                ?? string.Empty;

            services.AddHealthChecks()
                .AddSqlServer(connectionString: sqlConnection,
                              name: "sqlserver",
                              failureStatus: HealthStatus.Unhealthy,
                              tags: ["db", "sql", "ready"])
                .AddRedis(redisConnectionString: redisConnection,
                          name: "redis",
                          failureStatus: HealthStatus.Degraded,
                          tags: ["cache", "redis", "ready"])
                .AddCheck<SmtpHealthCheck>(name: "smtp",
                                           failureStatus: HealthStatus.Degraded,
                                           tags: ["email", "smtp"]);
        }
    }
}