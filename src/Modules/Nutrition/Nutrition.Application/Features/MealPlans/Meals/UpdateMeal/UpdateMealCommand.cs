namespace Nutrition.Application.Features.MealPlans.Meals.UpdateMeal
{
    public sealed record UpdateMealCommand(Guid MealPlanId, Guid MealId, string Name, int Order) : ICommand<Result<bool>>;
}
