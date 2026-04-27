namespace Nutrition.Application.Features.DailyNutritionLogs.UpdateDailyLog
{
    public sealed class UpdateDailyLogCommandValidator : AbstractValidator<UpdateDailyLogCommand>
    {
        public UpdateDailyLogCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.DailyCalorieGoal)
                .GreaterThan(0)
                .When(x => x.DailyCalorieGoal.HasValue);

            RuleFor(x => x.Note).MaximumLength(500);
        }
    }
}
