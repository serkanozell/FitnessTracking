namespace Nutrition.Application.Features.MealPlans.CreateMealPlan
{
    public sealed record CreateMealPlanCommand(
        string Name,
        DateTime Date,
        string? Note) : ICommand<Result<Guid>>;
}
