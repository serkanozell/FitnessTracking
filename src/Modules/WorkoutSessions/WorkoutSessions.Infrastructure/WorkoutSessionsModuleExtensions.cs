using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkoutSessions.Contracts;
using WorkoutSessions.Domain.Repositories;
using WorkoutSessions.Infrastructure.Persistence;
using WorkoutSessions.Infrastructure.Repositories;
using WorkoutSessions.Infrastructure.Services;

namespace WorkoutSessions.Infrastructure
{
    public static class WorkoutSessionsModuleExtensions
    {
        public static IServiceCollection AddWorkoutSessionsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddModuleDbContext<WorkoutSessionsDbContext>(configuration, WorkoutSessionsSchema.Name);

            services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
            services.AddScoped<IWorkoutSessionsUnitOfWork, WorkoutSessionsUnitOfWork>();
            services.AddScoped<IWorkoutSessionModule, WorkoutSessionModuleService>();

            return services;
        }
    }
}