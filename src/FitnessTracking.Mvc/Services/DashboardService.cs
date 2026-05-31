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

    public async Task<IReadOnlyList<VolumeTrendPointDto>> GetVolumeTrendAsync(int days = 30,
                                                                              AnalyticsGroupingPeriod period = AnalyticsGroupingPeriod.Day,
                                                                              Guid? workoutProgramId = null,
                                                                              Guid? workoutProgramSplitId = null,
                                                                              CancellationToken cancellationToken = default)
    {
        var url = $"{AnalyticsUrl}/volume-trend?days={days}&period={(int)period}";
        if (workoutProgramId.HasValue && workoutProgramId.Value != Guid.Empty)
            url += $"&workoutProgramId={workoutProgramId.Value}";
        if (workoutProgramSplitId.HasValue && workoutProgramSplitId.Value != Guid.Empty)
            url += $"&workoutProgramSplitId={workoutProgramSplitId.Value}";

        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<VolumeTrendPointDto>>(url, cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<ExerciseProgressPointDto>> GetExerciseProgressAsync(Guid exerciseId,
                                                                                        int days = 90,
                                                                                        CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<ExerciseProgressPointDto>>(
            $"{AnalyticsUrl}/exercise-progress/{exerciseId}?days={days}", cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<MuscleGroupVolumeDto>> GetMuscleGroupDistributionAsync(int days = 30,
                                                                                           CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<MuscleGroupVolumeDto>>(
            $"{AnalyticsUrl}/muscle-group-distribution?days={days}", cancellationToken);
        return result ?? [];
    }

    public async Task<IReadOnlyList<PersonalRecordDto>> GetPersonalRecordsAsync(int top = 10,
                                                                                CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<PersonalRecordDto>>(
            $"{AnalyticsUrl}/personal-records?top={top}", cancellationToken);
        return result ?? [];
    }
}
