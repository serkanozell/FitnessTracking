namespace Nutrition.Application.Features.MealPlans.UpdateMealPlan
{
    public sealed record UpdateMealPlanCommand(Guid Id, string Name, DateTime Date, string? Note) : ICommand<Result<bool>>;
}
