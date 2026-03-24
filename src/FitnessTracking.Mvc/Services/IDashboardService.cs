using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface IDashboardService
{
    Task<DashboardDto?> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeightTrendDto>> GetWeightTrendAsync(int days = 90, CancellationToken cancellationToken = default);
}
