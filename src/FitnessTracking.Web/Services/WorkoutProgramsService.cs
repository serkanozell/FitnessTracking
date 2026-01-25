using FitnessTracking.Web.Models;
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
        return await _httpClient.GetFromJsonAsync<WorkoutProgramDto>(
            $"{BaseUrl}/{id}",
            cancellationToken);
    }

    public async Task<IReadOnlyList<WorkoutProgramExerciseDto>> GetProgramExercisesAsync(
        Guid programId,
        CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IReadOnlyList<WorkoutProgramExerciseDto>>(
            $"{BaseUrl}/{programId}/exercises",
            cancellationToken);

        return result ?? Array.Empty<WorkoutProgramExerciseDto>();
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

    public async Task<Guid> AddExerciseAsync(
        Guid programId,
        AddProgramExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/{programId}/exercises",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var workoutProgramExerciseId = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
        return workoutProgramExerciseId;
    }

    public async Task UpdateExerciseAsync(
        Guid programId,
        Guid workoutProgramExerciseId,
        UpdateProgramExerciseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseUrl}/{programId}/exercises/{workoutProgramExerciseId}",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveExerciseAsync(
        Guid programId,
        Guid workoutProgramExerciseId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseUrl}/{programId}/exercises/{workoutProgramExerciseId}",
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}