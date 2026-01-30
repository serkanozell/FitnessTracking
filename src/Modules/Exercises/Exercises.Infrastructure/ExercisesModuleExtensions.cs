using Exercises.Application.Services;
using Exercises.Domain.Repositories;
using Exercises.Infrastructure.Persistance;
using Exercises.Infrastructure.Repositories;
using Exercises.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exercises.Infrastructure
{
    public static class ExercisesModuleExtensions
    {
        public static IServiceCollection AddExercisesInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("FitnessDbConnection");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

            services.AddDbContext<ExercisesDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IDomainEventDispatcher, ExercisesDomainEventDispatcher>();

            // Repositories
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IExercisesUnitOfWork, ExercisesUnitOfWork>();

            // Modüller arası servisler
            services.AddScoped<IExerciseReadService, ExerciseReadService>();

            return services;
        }
    }
}