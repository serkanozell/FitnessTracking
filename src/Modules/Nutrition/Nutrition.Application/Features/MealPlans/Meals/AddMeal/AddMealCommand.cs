namespace Nutrition.Application.Features.MealPlans.Meals.AddMeal
{
    public sealed record AddMealCommand(Guid MealPlanId, string Name, int Order, string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}
