namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.AddMealItem
{
    public sealed record AddMealItemCommand(
        Guid MealPlanId,
        Guid MealId,
        Guid FoodId,
        decimal Quantity,
        string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}
