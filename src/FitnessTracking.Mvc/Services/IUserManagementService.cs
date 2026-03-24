using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface IUserManagementService
{
    // Users
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateUserAsync(CreateUserModel model, CancellationToken cancellationToken = default);
    Task<bool> ActivateUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    // Roles
    Task<IReadOnlyList<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<Guid> CreateRoleAsync(RoleEditModel model, CancellationToken cancellationToken = default);
    Task<bool> UpdateRoleAsync(Guid id, RoleEditModel model, CancellationToken cancellationToken = default);
}
