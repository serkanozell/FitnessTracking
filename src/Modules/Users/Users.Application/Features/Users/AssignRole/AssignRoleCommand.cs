namespace Users.Application.Features.Users.AssignRole
{
    public sealed record AssignRoleCommand(Guid UserId, Guid RoleId) : ICommand<Result>;
}
