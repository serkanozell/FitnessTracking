namespace Nutrition.Application.Features.MealPlans.Meals.RemoveMeal
{
    public sealed record RemoveMealCommand(Guid MealPlanId, Guid MealId) : ICommand<Result<bool>>;
}
