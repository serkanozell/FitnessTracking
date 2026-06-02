namespace Users.Application.Features.Users.AssignRole
{
    public sealed record AssignRoleCommand(Guid UserId, Guid RoleId, string? IdempotencyKey = null) : ICommand<Result>, IIdempotentCommand;
}
