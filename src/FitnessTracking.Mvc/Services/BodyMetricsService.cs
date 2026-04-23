using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class BodyMetricsService(HttpClient httpClient) : IBodyMetricsService
{
    private const string BaseUrl = "api/v1/body-metrics";

    public async Task<PagedResult<BodyMetricDto>> GetPagedAsync(
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<PagedResult<BodyMetricDto>>(
            $"{BaseUrl}?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken);
        return result ?? new PagedResult<BodyMetricDto>();
    }

    public async Task<BodyMetricDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BodyMetricDto>(cancellationToken: cancellationToken);
    }

    public async Task<Guid> CreateAsync(BodyMetricEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model.Date,
            model.Weight,
            model.Height,
            model.BodyFatPercentage,
            model.MuscleMass,
            model.WaistCircumference,
            model.ChestCircumference,
            model.ArmCircumference,
            model.HipCircumference,
            model.ThighCircumference,
            model.NeckCircumference,
            model.ShoulderCircumference,
            model.Note
        };
        using var response = await httpClient.PostAsJsonAsync(BaseUrl, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateBodyMetricResponse>(cancellationToken: cancellationToken);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateAsync(Guid id, BodyMetricEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model.Date,
            model.Weight,
            model.Height,
            model.BodyFatPercentage,
            model.MuscleMass,
            model.WaistCircumference,
            model.ChestCircumference,
            model.ArmCircumference,
            model.HipCircumference,
            model.ThighCircumference,
            model.NeckCircumference,
            model.ShoulderCircumference,
            model.Note
        };
        using var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", payload, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"{BaseUrl}/{id}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    private sealed record CreateBodyMetricResponse(Guid Id);
}
