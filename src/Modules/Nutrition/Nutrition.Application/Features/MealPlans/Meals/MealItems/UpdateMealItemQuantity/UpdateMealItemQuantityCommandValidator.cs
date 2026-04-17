namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.UpdateMealItemQuantity
{
    public sealed class UpdateMealItemQuantityCommandValidator : AbstractValidator<UpdateMealItemQuantityCommand>
    {
        public UpdateMealItemQuantityCommandValidator()
        {
            RuleFor(x => x.MealPlanId).NotEmpty();
            RuleFor(x => x.MealId).NotEmpty();
            RuleFor(x => x.MealItemId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
