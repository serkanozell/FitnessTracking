namespace FitnessTracking.Api.Configuration
{
    /// <summary>
    /// Strongly-typed binding for the "RateLimiting" configuration section.
    /// Holds the global (per-IP) fixed-window limit plus the named per-endpoint policies.
    /// All values have safe in-code defaults so the API works even without configuration.
    /// </summary>
    public sealed class RateLimitingOptions
    {
        public const string SectionName = "RateLimiting";

        /// <summary>
        /// Global limiter applied to every request, partitioned by client IP.
        /// Acts as a coarse backstop; named policies further constrain specific endpoints.
        /// </summary>
        public RateLimitRule Global { get; set; } = new()
        {
            PermitLimit = 100,
            WindowInSeconds = 60,
            QueueLimit = 0
        };

        /// <summary>
        /// Strict per-IP policy for authentication endpoints (login/register/refresh)
        /// to slow down credential brute-force and account-enumeration attempts.
        /// </summary>
        public RateLimitRule Authentication { get; set; } = new()
        {
            PermitLimit = 10,
            WindowInSeconds = 60,
            QueueLimit = 0
        };

        /// <summary>
        /// Generous per-user policy for the dashboard/analytics endpoints, which a single
        /// page load fans out into several concurrent calls. Partitioned by user id so one
        /// authenticated user's bursts never starve another user behind the same NAT/IP.
        /// </summary>
        public RateLimitRule Dashboard { get; set; } = new()
        {
            PermitLimit = 60,
            WindowInSeconds = 60,
            QueueLimit = 10
        };
    }
}
