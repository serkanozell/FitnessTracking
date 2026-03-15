namespace Users.Application.Features.Users.RevokeRefreshToken
{
    public sealed record RevokeRefreshTokenCommand(string RefreshToken) : ICommand<Result<bool>>;
}
