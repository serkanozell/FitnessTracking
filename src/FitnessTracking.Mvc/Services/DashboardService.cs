using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class DashboardService(HttpClient httpClient) : IDashboardService
{
    private const string BaseUrl = "api/v1/dashboard";

    public async Task<DashboardDto?> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<DashboardDto>(BaseUrl, cancellationToken);
    }

    public async Task<IReadOnlyList<WeightTrendDto>> GetWeightTrendAsync(int days = 90, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<WeightTrendDto>>(
            $"{BaseUrl}/weight-trend?days={days}", cancellationToken);
        return result ?? [];
    }
}
