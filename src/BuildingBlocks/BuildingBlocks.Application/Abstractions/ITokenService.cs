namespace BuildingBlocks.Application.Abstractions
{
    public interface ITokenService
    {
        /// <summary>
        /// Generate a JWT for the specified user id, email and roles.
        /// </summary>
        string GenerateToken(Guid userId, string? email, IEnumerable<string> roles);

        /// <summary>
        /// Refresh token expiration in days.
        /// </summary>
        int RefreshTokenExpirationDays { get; }
    }
}