namespace Nutrition.Application.Features.MealPlans.Meals.AddMeal
{
    public sealed record AddMealCommand(Guid MealPlanId, string Name, int Order) : ICommand<Result<Guid>>;
}
