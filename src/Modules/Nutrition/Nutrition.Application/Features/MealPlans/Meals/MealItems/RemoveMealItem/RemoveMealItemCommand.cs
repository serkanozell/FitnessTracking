namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.RemoveMealItem
{
    public sealed record RemoveMealItemCommand(Guid MealPlanId, Guid MealId, Guid MealItemId) : ICommand<Result<bool>>;
}
