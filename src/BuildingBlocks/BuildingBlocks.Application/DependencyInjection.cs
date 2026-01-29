using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;

namespace BuildingBlocks.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBuildingBlocksApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            });

            AddLogService(configuration);

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }

        private static void AddLogService(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                                                           .CreateLogger();
        }
    }
}