using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkoutPrograms.Contracts;
using WorkoutPrograms.Domain.Repositories;
using WorkoutPrograms.Infrastructure.Persistence;
using WorkoutPrograms.Infrastructure.Repositories;
using WorkoutPrograms.Infrastructure.Services;

namespace WorkoutPrograms.Infrastructure
{
    public static class WorkoutProgramsModuleExtensions
    {
        public static IServiceCollection AddWorkoutProgramsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddModuleDbContext<WorkoutProgramsDbContext>(configuration, WorkoutProgramsSchema.Name);

            // Repositories
            services.AddScoped<IWorkoutProgramRepository, WorkoutProgramRepository>();
            services.AddScoped<IWorkoutProgramsUnitOfWork, WorkoutProgramsUnitOfWork>();

            // Modüller arası servisler
            services.AddScoped<IWorkoutProgramModule, WorkoutProgramModuleService>();

            return services;
        }
    }
}