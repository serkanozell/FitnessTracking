namespace BuildingBlocks.Infrastructure.Security
{
    public sealed class JwtOptions
    {
        public string Key { get; set; } = default!;
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        /// <summary>Access token expiration in minutes. Default 180 (3 hours).</summary>
        public int ExpirationMinutes { get; set; } = 180;
        /// <summary>Refresh token expiration in days. Default 7.</summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}