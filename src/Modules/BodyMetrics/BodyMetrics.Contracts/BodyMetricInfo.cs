namespace BodyMetrics.Contracts;

public record LatestBodyMetricInfo(
    DateTime Date,
    decimal? Weight,
    decimal? BodyFatPercentage,
    decimal? MuscleMass);

public record WeightTrendPoint(DateTime Date, decimal Weight);