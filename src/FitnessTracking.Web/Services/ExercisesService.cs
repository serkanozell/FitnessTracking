using FitnessTracking.Web.Models;
using System.Net;
using System.Net.Http.Json;

public sealed class ExercisesService : IExercisesService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/exercises";

    public ExercisesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ExerciseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _httpClient
            .GetFromJsonAsync<IReadOnlyList<ExerciseDto>>(BaseUrl, cancellationToken);

        return result ?? Array.Empty<ExerciseDto>();
    }

    public async Task<ExerciseDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ExerciseDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        ExerciseEditModel model,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model.Name,
            model.MuscleGroup,
            model.Description
        };

        var response = await _httpClient.PostAsJsonAsync(BaseUrl, payload, cancellationToken);

        response.EnsureSuccessStatusCode();

        var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
        return id;
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        ExerciseEditModel model,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model.Name,
            model.MuscleGroup,
            model.Description
        };

        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseUrl}/{id}",
            payload,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseUrl}/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }
}