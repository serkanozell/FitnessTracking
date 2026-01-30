using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkoutPrograms.Domain.Repositories;
using WorkoutPrograms.Infrastructure.Persistance;
using WorkoutPrograms.Infrastructure.Repositories;

namespace WorkoutPrograms.Infrastructure
{
    public static class WorkoutProgramsModuleExtensions
    {
        public static IServiceCollection WorkoutProgramsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("FitnessDbConnection");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<WorkoutProgramsDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IDomainEventDispatcher, WorkoutProgramsDomainEventDispatcher>();

            // Repositories
            services.AddScoped<IWorkoutProgramRepository, WorkoutProgramRepository>();
            services.AddScoped<IWorkoutProgramsUnitOfWork, WorkoutProgramsUnitOfWork>();

            return services;
        }
    }
}
