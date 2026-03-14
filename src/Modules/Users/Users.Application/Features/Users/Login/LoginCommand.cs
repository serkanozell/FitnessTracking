namespace Users.Application.Features.Users.Login
{
    public sealed record LoginCommand(string Email, string Password) : ICommand<Result<LoginResponse>>;

    public sealed record LoginResponse(string Token, Guid UserId, string Email, List<string> Roles);
}
