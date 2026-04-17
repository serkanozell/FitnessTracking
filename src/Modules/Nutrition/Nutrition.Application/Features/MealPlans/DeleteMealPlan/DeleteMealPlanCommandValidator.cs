namespace Nutrition.Application.Features.MealPlans.DeleteMealPlan
{
    public sealed class DeleteMealPlanCommandValidator : AbstractValidator<DeleteMealPlanCommand>
    {
        public DeleteMealPlanCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
