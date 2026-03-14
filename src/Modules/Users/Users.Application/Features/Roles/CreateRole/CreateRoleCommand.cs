namespace Users.Application.Features.Roles.CreateRole
{
    public sealed record CreateRoleCommand(string Name, string? Description) : ICommand<Result<Guid>>;
}
