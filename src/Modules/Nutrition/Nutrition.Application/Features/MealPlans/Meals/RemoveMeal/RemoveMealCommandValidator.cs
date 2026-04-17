namespace Nutrition.Application.Features.MealPlans.Meals.RemoveMeal
{
    public sealed class RemoveMealCommandValidator : AbstractValidator<RemoveMealCommand>
    {
        public RemoveMealCommandValidator()
        {
            RuleFor(x => x.MealPlanId).NotEmpty();
            RuleFor(x => x.MealId).NotEmpty();
        }
    }
}
