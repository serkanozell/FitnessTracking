namespace BodyMetrics.Application.Features.BodyMetrics.CreateBodyMetric
{
    public sealed class CreateBodyMetricCommandValidator : AbstractValidator<CreateBodyMetricCommand>
    {
        public CreateBodyMetricCommandValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty();

            RuleFor(x => x.Weight)
                .GreaterThan(0).When(x => x.Weight.HasValue);

            RuleFor(x => x.Height)
                .GreaterThan(0).When(x => x.Height.HasValue);

            RuleFor(x => x.BodyFatPercentage)
                .InclusiveBetween(0, 100).When(x => x.BodyFatPercentage.HasValue);

            RuleFor(x => x.Note)
                .MaximumLength(500);
        }
    }
}