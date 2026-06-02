namespace Nutrition.Application.Features.MealPlans.CreateMealPlan
{
    public sealed record CreateMealPlanCommand(
        string Name,
        DateTime Date,
        string? Note,
        string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}
