namespace Nutrition.Application.Features.MealPlans.DeleteMealPlan
{
    public sealed record DeleteMealPlanCommand(Guid Id) : ICommand<Result<bool>>;
}
