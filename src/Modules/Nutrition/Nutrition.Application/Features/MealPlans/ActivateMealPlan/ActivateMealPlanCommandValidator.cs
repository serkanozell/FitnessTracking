namespace Nutrition.Application.Features.MealPlans.ActivateMealPlan
{
    public sealed class ActivateMealPlanCommandValidator : AbstractValidator<ActivateMealPlanCommand>
    {
        public ActivateMealPlanCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
