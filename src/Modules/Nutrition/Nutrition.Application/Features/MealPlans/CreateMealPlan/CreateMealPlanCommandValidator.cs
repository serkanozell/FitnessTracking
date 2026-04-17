namespace Nutrition.Application.Features.MealPlans.CreateMealPlan
{
    public sealed class CreateMealPlanCommandValidator : AbstractValidator<CreateMealPlanCommand>
    {
        public CreateMealPlanCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Date)
                .NotEmpty();

            RuleFor(x => x.Note)
                .MaximumLength(500);
        }
    }
}
