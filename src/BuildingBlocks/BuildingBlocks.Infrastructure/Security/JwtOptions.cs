namespace BuildingBlocks.Infrastructure.Security
{
    public sealed class JwtOptions
    {
        public string Key { get; set; } = default!;
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        /// <summary>Expiration in minutes. Default 180 (3 hours).</summary>
        public int ExpirationMinutes { get; set; } = 180;
    }
}