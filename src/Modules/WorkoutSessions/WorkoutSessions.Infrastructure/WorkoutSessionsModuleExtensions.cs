using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkoutSessions.Domain.Repositories;
using WorkoutSessions.Infrastructure.Persistence;
using WorkoutSessions.Infrastructure.Repositories;

namespace WorkoutSessions.Infrastructure
{
    public static class WorkoutSessionsModuleExtensions
    {
        public static IServiceCollection WorkoutSessionsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("FitnessDbConnection");

            services.AddDbContext<WorkoutSessionsDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            // Repositories
            services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
            services.AddScoped<IWorkoutSessionsUnitOfWork, WorkoutSessionsUnitOfWork>();

            return services;
        }
    }
}