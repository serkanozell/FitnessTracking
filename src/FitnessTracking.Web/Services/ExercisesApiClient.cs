using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Web.Models;

namespace FitnessTracking.Web.Services
{
    public sealed class ExercisesApiClient
    {
        private readonly HttpClient _httpClient;

        public ExercisesApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: api/exercises
        public async Task<IReadOnlyList<ExerciseDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _httpClient
                .GetFromJsonAsync<IReadOnlyList<ExerciseDto>>("api/exercises", cancellationToken);

            return result ?? Array.Empty<ExerciseDto>();
        }

        // GET: api/exercises/{id}
        public async Task<ExerciseDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/exercises/{id}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ExerciseDto>(cancellationToken: cancellationToken);
        }

        // POST: api/exercises
        // Controller: 201 + body: Guid
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

            var response = await _httpClient.PostAsJsonAsync("api/exercises", payload, cancellationToken);

            response.EnsureSuccessStatusCode();

            var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
            return id;
        }

        // PUT: api/exercises/{id}
        // 204 NoContent, 404 NotFound
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

            var response = await _httpClient.PutAsJsonAsync($"api/exercises/{id}", payload, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        // DELETE: api/exercises/{id}
        // 204 NoContent, 404 NotFound
        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/exercises/{id}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}