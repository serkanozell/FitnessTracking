namespace Users.Application.Features.Users.CreateUser
{
    public sealed record CreateUserCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        List<Guid> RoleIds) : ICommand<Result<Guid>>;
}