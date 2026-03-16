using BodyMetrics.Contracts;
using BodyMetrics.Domain.Repositories;
using BodyMetrics.Infrastructure.Persistence;
using BodyMetrics.Infrastructure.Repositories;
using BodyMetrics.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BodyMetrics.Infrastructure
{
    public static class BodyMetricsModuleExtensions
    {
        public static IServiceCollection AddBodyMetricsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("FitnessDbConnection");

            services.AddDbContext<BodyMetricsDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IBodyMetricRepository, BodyMetricRepository>();
            services.AddScoped<IBodyMetricsUnitOfWork, BodyMetricsUnitOfWork>();
            services.AddScoped<IBodyMetricModule, BodyMetricModuleService>();

            return services;
        }
    }
}