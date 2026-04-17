namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.UpdateMealItemQuantity
{
    public sealed record UpdateMealItemQuantityCommand(
        Guid MealPlanId,
        Guid MealId,
        Guid MealItemId,
        decimal Quantity) : ICommand<Result<bool>>;
}
