namespace BodyMetrics.Application.Features.BodyMetrics.ActivateBodyMetric
{
    public sealed class ActivateBodyMetricCommandValidator : AbstractValidator<ActivateBodyMetricCommand>
    {
        public ActivateBodyMetricCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}