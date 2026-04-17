namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.AddMealItem
{
    public sealed class AddMealItemCommandValidator : AbstractValidator<AddMealItemCommand>
    {
        public AddMealItemCommandValidator()
        {
            RuleFor(x => x.MealPlanId).NotEmpty();
            RuleFor(x => x.MealId).NotEmpty();
            RuleFor(x => x.FoodId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
