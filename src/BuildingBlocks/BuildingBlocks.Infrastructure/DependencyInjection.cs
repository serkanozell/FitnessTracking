using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure.Persistence.Caching;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<ICacheAsideService, CacheAsideService>();

            return services;
        }
    }
}