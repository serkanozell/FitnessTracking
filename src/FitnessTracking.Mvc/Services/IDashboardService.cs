using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface IDashboardService
{
    Task<DashboardDto?> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeightTrendDto>> GetWeightTrendAsync(int days = 90, CancellationToken cancellationToken = default);

    // All analytics-page datasets (volume trend, muscle-group distribution, exercise
    // progress, personal records) are fetched in a single aggregate call to avoid
    // HTTP fan-out from the MVC layer. See ROADMAP P10.
    Task<AnalyticsPageDto?> GetAnalyticsPageAsync(int days = 30,
                                                  AnalyticsGroupingPeriod period = AnalyticsGroupingPeriod.Day,
                                                  Guid? exerciseId = null,
                                                  Guid? programId = null,
                                                  Guid? splitId = null,
                                                  CancellationToken cancellationToken = default);
}
