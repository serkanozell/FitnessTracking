namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.UpdateLogEntryQuantity
{
    public sealed class UpdateLogEntryQuantityCommandValidator : AbstractValidator<UpdateLogEntryQuantityCommand>
    {
        public UpdateLogEntryQuantityCommandValidator()
        {
            RuleFor(x => x.DailyNutritionLogId).NotEmpty();
            RuleFor(x => x.EntryId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
