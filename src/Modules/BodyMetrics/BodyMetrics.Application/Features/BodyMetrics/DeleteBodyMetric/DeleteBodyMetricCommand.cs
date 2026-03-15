namespace BodyMetrics.Application.Features.BodyMetrics.DeleteBodyMetric
{
    public sealed record DeleteBodyMetricCommand(Guid Id) : ICommand<Result<bool>>;
}