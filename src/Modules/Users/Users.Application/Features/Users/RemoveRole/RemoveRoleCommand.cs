namespace Users.Application.Features.Users.RemoveRole
{
    public sealed record RemoveRoleCommand(Guid UserId, Guid RoleId) : ICommand<Result>;
}
