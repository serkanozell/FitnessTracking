using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class WorkoutSessionsService(HttpClient httpClient) : IWorkoutSessionsService
{
    private const string BaseUrl = "api/v1/workout-sessions";

    public async Task<PagedResult<WorkoutSessionDto>> GetPagedAsync(
        Guid? programId = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}?pageNumber={pageNumber}&pageSize={pageSize}";
        if (programId.HasValue)
            url += $"&programId={programId.Value}";

        using var result = await httpClient.GetAsync(url, cancellationToken);

        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<PagedResult<WorkoutSessionDto>>(cancellationToken: cancellationToken)
               ?? new PagedResult<WorkoutSessionDto>();
    }

    public async Task<WorkoutSessionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkoutSessionDto>(cancellationToken: cancellationToken);
    }

    public async Task<Guid> CreateAsync(WorkoutSessionEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.WorkoutProgramId, model.WorkoutProgramSplitId, model.Date };
        using var response = await httpClient.PostAsJsonAsync(BaseUrl, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<WorkoutSessionDto>(cancellationToken: cancellationToken);
        return result!.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, WorkoutSessionEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.Date };
        using var response = await httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", payload, cancellationToken);
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

    public async Task<WorkoutSessionDetailsDto?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<WorkoutSessionDetailsDto>(cancellationToken: cancellationToken);
    }

    public async Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"{BaseUrl}/{id}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ActivateWorkoutExerciseAsync(Guid sessionId, Guid workoutExerciseId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"{BaseUrl}/{sessionId}/exercises/{workoutExerciseId}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<IReadOnlyList<WorkoutExerciseDto>> GetWorkoutExercisesAsync(
        Guid sessionId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"{BaseUrl}/{sessionId}/exercises", cancellationToken);
        if (!response.IsSuccessStatusCode) return [];
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<WorkoutExerciseDto>>(cancellationToken: cancellationToken);
        return result ?? [];
    }

    public async Task<Guid> AddWorkoutExerciseAsync(
        Guid sessionId, WorkoutExerciseEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { ExerciseId = model.ExerciseId, model.SetNumber, model.Weight, model.Reps };
        using var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/{sessionId}/exercises", payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<WorkoutExerciseAddResult>(cancellationToken: cancellationToken);
        return result!.SessionExerciseId;
    }

    public async Task<bool> UpdateWorkoutExerciseAsync(
        Guid sessionId, Guid workoutExerciseId, WorkoutExerciseEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.SetNumber, model.Weight, model.Reps };
        using var response = await httpClient.PutAsJsonAsync(
            $"{BaseUrl}/{sessionId}/exercises/{workoutExerciseId}", payload, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteWorkoutExerciseAsync(
        Guid sessionId, Guid workoutExerciseId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(
            $"{BaseUrl}/{sessionId}/exercises/{workoutExerciseId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }
}
