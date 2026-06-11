using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class DashboardService(HttpClient httpClient) : IDashboardService
{
    private const string BaseUrl = "api/v1/dashboard";
    private const string AnalyticsUrl = "api/v1/dashboard/analytics";

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

    public async Task<AnalyticsPageDto?> GetAnalyticsPageAsync(int days = 30,
                                                               AnalyticsGroupingPeriod period = AnalyticsGroupingPeriod.Day,
                                                               Guid? exerciseId = null,
                                                               Guid? programId = null,
                                                               Guid? splitId = null,
                                                               CancellationToken cancellationToken = default)
    {
        var url = $"{AnalyticsUrl}/page?days={days}&period={(int)period}";
        if (exerciseId.HasValue && exerciseId.Value != Guid.Empty)
            url += $"&exerciseId={exerciseId.Value}";
        if (programId.HasValue && programId.Value != Guid.Empty)
            url += $"&programId={programId.Value}";
        if (splitId.HasValue && splitId.Value != Guid.Empty)
            url += $"&splitId={splitId.Value}";

        return await httpClient.GetFromJsonAsync<AnalyticsPageDto>(url, cancellationToken);
    }
}
