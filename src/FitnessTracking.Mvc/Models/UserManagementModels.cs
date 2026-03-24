namespace FitnessTracking.Mvc.Models;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public List<string> Roles { get; init; } = [];
    public DateTime? CreatedDate { get; init; }
}

public sealed class CreateUserModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<Guid> RoleIds { get; set; } = [];
}

public sealed class RoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public DateTime? CreatedDate { get; init; }
}

public sealed class RoleEditModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
