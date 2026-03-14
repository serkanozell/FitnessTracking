namespace Users.Application.Features.Users.Register
{
    public sealed record RegisterCommand(string Email, string Password, string FirstName, string LastName) : ICommand<Result<Guid>>;
}
