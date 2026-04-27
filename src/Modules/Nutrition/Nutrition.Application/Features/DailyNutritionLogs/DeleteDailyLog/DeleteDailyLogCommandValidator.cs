namespace Nutrition.Application.Features.DailyNutritionLogs.DeleteDailyLog
{
    public sealed class DeleteDailyLogCommandValidator : AbstractValidator<DeleteDailyLogCommand>
    {
        public DeleteDailyLogCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
