namespace Nutrition.Application.Features.DailyNutritionLogs.CreateDailyLog
{
    public sealed class CreateDailyLogCommandValidator : AbstractValidator<CreateDailyLogCommand>
    {
        public CreateDailyLogCommandValidator()
        {
            RuleFor(x => x.Date).NotEmpty();

            RuleFor(x => x.DailyCalorieGoal)
                .GreaterThan(0)
                .When(x => x.DailyCalorieGoal.HasValue);

            RuleFor(x => x.Note).MaximumLength(500);
        }
    }
}
