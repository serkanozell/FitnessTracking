namespace Users.Application.Features.Roles.CreateRole
{
    public sealed record CreateRoleCommand(string Name, string? Description, string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}
