namespace BodyMetrics.Application.Features.BodyMetrics.DeleteBodyMetric
{
    public sealed class DeleteBodyMetricCommandValidator : AbstractValidator<DeleteBodyMetricCommand>
    {
        public DeleteBodyMetricCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}