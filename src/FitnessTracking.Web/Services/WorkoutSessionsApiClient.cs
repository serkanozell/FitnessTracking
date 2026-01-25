using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Web.Models;

namespace FitnessTracking.Web.Services
{
    public sealed class WorkoutSessionsApiClient
    {
        private readonly HttpClient _httpClient;

        public WorkoutSessionsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: api/workoutsessions
        public async Task<IReadOnlyList<WorkoutSessionDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _httpClient
                .GetFromJsonAsync<IReadOnlyList<WorkoutSessionDto>>(
                    "api/workoutsessions",
                    cancellationToken);

            return result ?? Array.Empty<WorkoutSessionDto>();
        }

        // GET: api/workoutsessions/{id}
        public async Task<WorkoutSessionDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/workoutsessions/{id}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<WorkoutSessionDto>(cancellationToken: cancellationToken);
        }

        // POST: api/workoutsessions
        // Controller: 201 + body: Guid
        public async Task<Guid> CreateAsync(
            WorkoutSessionEditModel model,
            CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                model.WorkoutProgramId,
                model.Date
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/workoutsessions",
                payload,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
            return id;
        }

        // PUT: api/workoutsessions/{id}
        // 204 / 404
        public async Task<bool> UpdateAsync(
            Guid id,
            WorkoutSessionEditModel model,
            CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                model.Date
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"api/workoutsessions/{id}",
                payload,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        // DELETE: api/workoutsessions/{id}
        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/workoutsessions/{id}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<Guid> AddWorkoutExerciseAsync(Guid sessionId, object request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/workoutsessions/{sessionId}/exercises",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            // controller CreatedAtAction body: workoutExerciseId (Guid)
            var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
            return id;
        }

        public async Task<WorkoutSessionDetailsDto?> GetDetailsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/workoutsessions/{id}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<WorkoutSessionDetailsDto>(
                cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<WorkoutExerciseDto>> GetWorkoutExercisesAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            var url = $"api/workoutsessions/{sessionId}/exercises";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // log or surface a friendly message to the UI
                var msg = $"Failed to load exercises for session {sessionId}. " +
                          $"Status: {(int)response.StatusCode} {response.ReasonPhrase}";
                // e.g. _logger.LogError(msg);
                return Array.Empty<WorkoutExerciseDto>();
            }

            var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<WorkoutExerciseDto>>(cancellationToken: cancellationToken);
            return result ?? Array.Empty<WorkoutExerciseDto>();
        }

        public async Task<Guid> AddWorkoutExerciseAsync(
            Guid sessionId,
            WorkoutExerciseEditModel model,
            CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                ExerciseId = model.ExerciseId,
                model.SetNumber,
                model.Weight,
                model.Reps
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"api/workoutsessions/{sessionId}/exercises",
                payload,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var id = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
            return id;
        }

        public async Task<bool> UpdateWorkoutExerciseAsync(
            Guid sessionId,
            Guid workoutExerciseId,
            WorkoutExerciseEditModel model,
            CancellationToken cancellationToken = default)
        {
            var payload = new
            {
                model.SetNumber,
                model.Weight,
                model.Reps
            };

            var response = await _httpClient.PutAsJsonAsync(
                $"api/workoutsessions/{sessionId}/exercises/{workoutExerciseId}",
                payload,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> DeleteWorkoutExerciseAsync(
            Guid sessionId,
            Guid workoutExerciseId,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/workoutsessions/{sessionId}/exercises/{workoutExerciseId}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}