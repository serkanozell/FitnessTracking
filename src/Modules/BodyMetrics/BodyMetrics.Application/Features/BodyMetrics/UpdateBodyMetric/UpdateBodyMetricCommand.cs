namespace BodyMetrics.Application.Features.BodyMetrics.UpdateBodyMetric
{
    public sealed record UpdateBodyMetricCommand(Guid Id,
                                                 DateTime Date,
                                                 decimal? Weight,
                                                 decimal? Height,
                                                 decimal? BodyFatPercentage,
                                                 decimal? MuscleMass,
                                                 decimal? WaistCircumference,
                                                 decimal? ChestCircumference,
                                                 decimal? ArmCircumference,
                                                 decimal? HipCircumference,
                                                 decimal? ThighCircumference,
                                                 decimal? NeckCircumference,
                                                 string? Note) : ICommand<Result<bool>>;
}