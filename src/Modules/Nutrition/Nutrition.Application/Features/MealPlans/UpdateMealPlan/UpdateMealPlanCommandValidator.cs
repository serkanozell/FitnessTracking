namespace Nutrition.Application.Features.MealPlans.UpdateMealPlan
{
    public sealed class UpdateMealPlanCommandValidator : AbstractValidator<UpdateMealPlanCommand>
    {
        public UpdateMealPlanCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Note).MaximumLength(500);
        }
    }
}
