using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class UserManagementService(HttpClient httpClient) : IUserManagementService
{
    // === Users ===

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/users/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: cancellationToken);
    }

    public async Task<Guid> CreateUserAsync(CreateUserModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.Email, model.Password, model.FirstName, model.LastName, model.RoleIds };
        using var response = await httpClient.PostAsJsonAsync("api/v1/users", payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>(cancellationToken: cancellationToken);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> ActivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"api/v1/users/{id}/activate", null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/users/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var payload = new { RoleId = roleId };
        using var response = await httpClient.PostAsJsonAsync($"api/v1/users/{userId}/roles", payload, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/users/{userId}/roles/{roleId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    // === Roles ===

    public async Task<IReadOnlyList<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<IReadOnlyList<RoleDto>>("api/v1/roles", cancellationToken);
        return result ?? [];
    }

    public async Task<Guid> CreateRoleAsync(RoleEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.Name, model.Description };
        using var response = await httpClient.PostAsJsonAsync("api/v1/roles", payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateRoleResponse>(cancellationToken: cancellationToken);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateRoleAsync(Guid id, RoleEditModel model, CancellationToken cancellationToken = default)
    {
        var payload = new { model.Name, model.Description };
        using var response = await httpClient.PutAsJsonAsync($"api/v1/roles/{id}", payload, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    private sealed record CreateUserResponse(Guid Id);
    private sealed record CreateRoleResponse(Guid Id);
}
