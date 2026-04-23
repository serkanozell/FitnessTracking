namespace Nutrition.Application.Features.MealPlans.Meals.UpdateMeal
{
    public sealed class UpdateMealCommandValidator : AbstractValidator<UpdateMealCommand>
    {
        public UpdateMealCommandValidator()
        {
            RuleFor(x => x.MealPlanId).NotEmpty();
            RuleFor(x => x.MealId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Order).GreaterThan(0);
        }
    }
}
