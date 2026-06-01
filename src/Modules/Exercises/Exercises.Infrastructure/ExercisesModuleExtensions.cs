using BuildingBlocks.Infrastructure.Persistence;
using Exercises.Contracts;
using Exercises.Domain.Repositories;
using Exercises.Infrastructure.Persistence;
using Exercises.Infrastructure.Repositories;
using Exercises.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Exercises.Infrastructure
{
    public static class ExercisesModuleExtensions
    {
        public static IServiceCollection AddExercisesInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddModuleDbContext<ExercisesDbContext>(configuration, ExercisesSchema.Name);

            // Repositories
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IExercisesUnitOfWork, ExercisesUnitOfWork>();

            // Modüller arası servisler
            services.AddScoped<IExerciseModule, ExerciseModuleService>();

            return services;
        }
    }
}