using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Infrastructure.Persistence.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
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