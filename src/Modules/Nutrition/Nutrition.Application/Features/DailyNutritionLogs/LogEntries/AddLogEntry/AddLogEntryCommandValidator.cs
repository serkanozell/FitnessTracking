namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.AddLogEntry
{
    public sealed class AddLogEntryCommandValidator : AbstractValidator<AddLogEntryCommand>
    {
        public AddLogEntryCommandValidator()
        {
            RuleFor(x => x.DailyNutritionLogId).NotEmpty();
            RuleFor(x => x.FoodId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
