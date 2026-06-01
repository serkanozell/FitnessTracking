using BodyMetrics.Contracts;
using BodyMetrics.Domain.Repositories;
using BodyMetrics.Infrastructure.Persistence;
using BodyMetrics.Infrastructure.Repositories;
using BodyMetrics.Infrastructure.Services;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BodyMetrics.Infrastructure
{
    public static class BodyMetricsModuleExtensions
    {
        public static IServiceCollection AddBodyMetricsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddModuleDbContext<BodyMetricsDbContext>(configuration, BodyMetricsSchema.Name);

            services.AddScoped<IBodyMetricRepository, BodyMetricRepository>();
            services.AddScoped<IBodyMetricsUnitOfWork, BodyMetricsUnitOfWork>();
            services.AddScoped<IBodyMetricModule, BodyMetricModuleService>();

            return services;
        }
    }
}