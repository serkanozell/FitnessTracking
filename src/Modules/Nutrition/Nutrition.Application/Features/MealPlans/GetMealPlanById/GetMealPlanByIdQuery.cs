using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.MealPlans.GetMealPlanById
{
    public sealed record GetMealPlanByIdQuery(Guid Id) : IQuery<Result<MealPlanDto>>;
}
