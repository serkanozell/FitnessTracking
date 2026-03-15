namespace BodyMetrics.Application.Features.BodyMetrics.ActivateBodyMetric
{
    public sealed record ActivateBodyMetricCommand(Guid Id) : ICommand<Result<Guid>>;
}