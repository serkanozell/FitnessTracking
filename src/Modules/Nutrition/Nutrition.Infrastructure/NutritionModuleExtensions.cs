using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nutrition.Contracts;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence;
using Nutrition.Infrastructure.Repositories;
using Nutrition.Infrastructure.Services;

namespace Nutrition.Infrastructure
{
    public static class NutritionModuleExtensions
    {
        public static IServiceCollection AddNutritionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("FitnessDbConnection");

            services.AddDbContext<NutritionDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddScoped<IMealPlanRepository, MealPlanRepository>();
            services.AddScoped<IDailyNutritionLogRepository, DailyNutritionLogRepository>();
            services.AddScoped<INutritionUnitOfWork, NutritionUnitOfWork>();
            services.AddScoped<INutritionModule, NutritionModuleService>();

            return services;
        }
    }
}
