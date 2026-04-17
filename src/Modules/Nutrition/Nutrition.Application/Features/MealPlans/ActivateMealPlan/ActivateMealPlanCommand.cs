namespace Nutrition.Application.Features.MealPlans.ActivateMealPlan
{
    public sealed record ActivateMealPlanCommand(Guid Id) : ICommand<Result<Guid>>;
}
