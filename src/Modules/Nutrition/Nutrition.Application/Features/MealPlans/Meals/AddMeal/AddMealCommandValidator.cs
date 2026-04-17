namespace Nutrition.Application.Features.MealPlans.Meals.AddMeal
{
    public sealed class AddMealCommandValidator : AbstractValidator<AddMealCommand>
    {
        public AddMealCommandValidator()
        {
            RuleFor(x => x.MealPlanId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Order).GreaterThan(0);
        }
    }
}
