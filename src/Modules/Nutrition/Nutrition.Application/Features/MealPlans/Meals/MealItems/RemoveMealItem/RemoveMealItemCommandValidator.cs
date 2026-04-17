namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.RemoveMealItem
{
    public sealed class RemoveMealItemCommandValidator : AbstractValidator<RemoveMealItemCommand>
    {
        public RemoveMealItemCommandValidator()
        {
            RuleFor(x => x.MealPlanId).NotEmpty();
            RuleFor(x => x.MealId).NotEmpty();
            RuleFor(x => x.MealItemId).NotEmpty();
        }
    }
}
