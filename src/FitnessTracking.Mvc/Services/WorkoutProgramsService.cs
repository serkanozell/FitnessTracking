using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class WorkoutProgramsService(HttpClient httpClient) : IWorkoutProgramsService
{
    private const string BaseUrl = "api/v1/workout-programs";

    public async Task<PagedResult<WorkoutProgramDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<PagedResult<WorkoutProgramDto>>(
            $"{BaseUrl}?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken);
        return result ?? new PagedResult<WorkoutProgramDto>();
    }

    public async Task<WorkoutProgramDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkoutProgramDto>(cancellationToken: cancellationToken);
    }

    public async Task<WorkoutProgramDetailViewDto?> GetDetailViewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"{BaseUrl}/{id}/detail-view", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkoutProgramDetailViewDto>(cancellationToken: cancellationToken);
    }

    public async Task<Guid> CreateAsync(CreateWorkoutProgramRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<IdResponse>(cancellationToken: cancellationToken);
        return result?.Id ?? Guid.Empty;
    }

    public async Task UpdateAsync(Guid id, UpdateWorkoutProgramRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"{BaseUrl}/{id}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    // Splits

    public async Task<IReadOnlyList<WorkoutProgramSplitDto>> GetSplitsAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<WorkoutProgramSplitDto>>(
            $"{BaseUrl}/{programId}/splits", cancellationToken);
        return result ?? [];
    }

    public async Task<Guid> AddSplitAsync(Guid programId, AddSplitRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{programId}/splits", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SplitIdResponse>(cancellationToken: cancellationToken);
        return result?.SplitId ?? Guid.Empty;
    }

    public async Task<bool> UpdateSplitAsync(Guid programId, Guid splitId, UpdateSplitRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{programId}/splits/{splitId}", request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ActivateSplitAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"{BaseUrl}/{programId}/splits/{splitId}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteSplitAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"{BaseUrl}/{programId}/splits/{splitId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    // Split Exercises

    public async Task<IReadOnlyList<WorkoutProgramExerciseDto>> GetSplitExercisesAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<WorkoutProgramExerciseDto>>(
            $"{BaseUrl}/{programId}/splits/{splitId}/exercises", cancellationToken);
        return result ?? [];
    }

    public async Task<Guid> AddExerciseToSplitAsync(Guid programId, Guid splitId, AddProgramExerciseRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{programId}/splits/{splitId}/exercises", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ExerciseIdResponse>(cancellationToken: cancellationToken);
        return result?.ExerciseId ?? Guid.Empty;
    }

    public async Task<bool> UpdateExerciseInSplitAsync(Guid programId, Guid splitId, Guid exerciseId, UpdateProgramExerciseRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{programId}/splits/{splitId}/exercises/{exerciseId}", request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ActivateSplitExerciseAsync(Guid programId, Guid splitId, Guid exerciseId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"{BaseUrl}/{programId}/splits/{splitId}/exercises/{exerciseId}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoveExerciseFromSplitAsync(Guid programId, Guid splitId, Guid exerciseId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"{BaseUrl}/{programId}/splits/{splitId}/exercises/{exerciseId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    private sealed record IdResponse(Guid Id);
    private sealed record SplitIdResponse(Guid SplitId);
    private sealed record ExerciseIdResponse(Guid ExerciseId);
}
