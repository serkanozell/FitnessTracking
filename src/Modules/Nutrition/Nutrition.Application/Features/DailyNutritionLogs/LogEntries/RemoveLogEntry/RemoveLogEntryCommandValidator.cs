namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.RemoveLogEntry
{
    public sealed class RemoveLogEntryCommandValidator : AbstractValidator<RemoveLogEntryCommand>
    {
        public RemoveLogEntryCommandValidator()
        {
            RuleFor(x => x.DailyNutritionLogId).NotEmpty();
            RuleFor(x => x.EntryId).NotEmpty();
        }
    }
}
