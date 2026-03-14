namespace Users.Application.Features.Roles.UpdateRole
{
    public sealed record UpdateRoleCommand(Guid RoleId, string Name, string? Description) : ICommand<Result>;
}
