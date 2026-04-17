using BuildingBlocks.Infrastructure.Persistence;
using Nutrition.Domain.Repositories;

namespace Nutrition.Infrastructure.Persistence
{
    public sealed class NutritionUnitOfWork(NutritionDbContext context) : UnitOfWork<NutritionDbContext>(context), INutritionUnitOfWork;
}
