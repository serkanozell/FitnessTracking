using FitnessTracking.Web.Models;
using System.Net;
using System.Net.Http.Json;

public sealed class WorkoutProgramsService : IWorkoutProgramsService
{
    private readonly HttpClient _httpClient;

    public WorkoutProgramsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private const string BaseUrl = "api/workoutprograms";

    public async Task<IReadOnlyList<WorkoutProgramDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<WorkoutProgramDto>>(
            BaseUrl,
            cancellationToken);

        return result ?? Array.Empty<WorkoutProgramDto>();
    }

    public async Task<WorkoutProgramDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<WorkoutProgramDto>($"{BaseUrl}/{id}", cancellationToken);
        return result;
    }

    public async Task<Guid> CreateAsync(CreateWorkoutProgramRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
        return id;
    }

    public async Task UpdateAsync(Guid id, UpdateWorkoutProgramRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
        
    // ---------- Splits ----------

    public async Task<IReadOnlyList<WorkoutProgramSplitDto>> GetSplitsAsync(
        Guid programId,
        CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<WorkoutProgramSplitDto>>(
            $"{BaseUrl}/{programId}/splits",
            cancellationToken);

        return result ?? Array.Empty<WorkoutProgramSplitDto>();
    }

    public async Task<Guid> AddSplitAsync(
        Guid programId,
        AddSplitRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/{programId}/splits",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
        return id;
    }

    public async Task<bool> UpdateSplitAsync(
        Guid programId,
        Guid splitId,
        UpdateSplitRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseUrl}/{programId}/splits/{splitId}",
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteSplitAsync(
        Guid programId,
        Guid splitId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseUrl}/{programId}/splits/{splitId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    // ---------- Split exercises ----------

    public async Task<IReadOnlyList<WorkoutProgramExerciseDto>> GetSplitExercisesAsync(
        Guid programId,
        Guid splitId,
        CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<WorkoutProgramExerciseDto>>(
            $"{BaseUrl}/{programId}/splits/{splitId}/exercises",
            cancellationToken);

        return result ?? Array.Empty<WorkoutProgramExerciseDto>();
    }

    public async Task<Guid> AddExerciseToSplitAsync(
        Guid programId,
        Guid splitId,
        AddProgramExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/{programId}/splits/{splitId}/exercises",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
        return id;
    }

    public async Task<bool> UpdateExerciseInSplitAsync(
        Guid programId,
        Guid splitId,
        Guid workoutProgramExerciseId,
        UpdateProgramExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseUrl}/{programId}/splits/{splitId}/exercises/{workoutProgramExerciseId}",
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoveExerciseFromSplitAsync(
        Guid programId,
        Guid splitId,
        Guid workoutProgramExerciseId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseUrl}/{programId}/splits/{splitId}/exercises/{workoutProgramExerciseId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }
}