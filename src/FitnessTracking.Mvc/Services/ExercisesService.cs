using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class ExercisesService(HttpClient httpClient) : IExercisesService
{
    private const string BaseUrl = "api/v1/exercises";

    public async Task<PagedResult<ExerciseDto>> GetPagedAsync(
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<PagedResult<ExerciseDto>>(
            $"{BaseUrl}?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken);
        return result ?? new PagedResult<ExerciseDto>();
    }

    public async Task<ExerciseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ExerciseDto>(cancellationToken: cancellationToken);
    }

    public async Task<Guid> CreateAsync(ExerciseEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.Name, model.PrimaryMuscleGroup, model.SecondaryMuscleGroup, model.Description };
        var response = await httpClient.PostAsJsonAsync(BaseUrl, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateExerciseResponse>(cancellationToken: cancellationToken);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateAsync(Guid id, ExerciseEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.Name, model.PrimaryMuscleGroup, model.SecondaryMuscleGroup, model.Description };
        var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", payload, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsync($"{BaseUrl}/{id}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    private sealed record CreateExerciseResponse(Guid Id);
}
