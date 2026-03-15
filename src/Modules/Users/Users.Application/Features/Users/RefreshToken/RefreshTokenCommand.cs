namespace Users.Application.Features.Users.RefreshToken
{
    public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<Result<RefreshTokenResponse>>;

    public sealed record RefreshTokenResponse(string Token, string RefreshToken, Guid UserId, string Email, List<string> Roles);
}
