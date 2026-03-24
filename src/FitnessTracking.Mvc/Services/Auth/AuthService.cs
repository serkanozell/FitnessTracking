using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services.Auth;

public sealed class AuthService(HttpClient httpClient) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequest request)
    {
        using var response = await httpClient.PostAsJsonAsync("api/v1/users/login", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        using var response = await httpClient.PostAsJsonAsync("api/v1/users/register", request);
        return response.IsSuccessStatusCode;
    }
}
