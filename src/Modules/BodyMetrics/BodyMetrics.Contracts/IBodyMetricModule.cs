namespace BodyMetrics.Contracts;

public interface IBodyMetricModule
{
    Task<LatestBodyMetricInfo?> GetLatestByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeightTrendPoint>> GetWeightTrendAsync(Guid userId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);
}