namespace FitnessTracking.Api.Configuration
{
    /// <summary>
    /// A single fixed-window rate-limit rule. Reused by the global limiter and every named policy
    /// so all limits share one configuration shape.
    /// </summary>
    public sealed class RateLimitRule
    {
        /// <summary>Maximum number of permitted requests within <see cref="WindowInSeconds"/>.</summary>
        public int PermitLimit { get; set; } = 100;

        /// <summary>Length of the fixed window in seconds.</summary>
        public int WindowInSeconds { get; set; } = 60;

        /// <summary>
        /// Number of requests queued once the permit limit is reached (processed oldest-first as
        /// permits free up). 0 means excess requests are rejected immediately with HTTP 429.
        /// </summary>
        public int QueueLimit { get; set; }

        /// <summary>The configured window as a <see cref="TimeSpan"/>.</summary>
        public TimeSpan Window => TimeSpan.FromSeconds(WindowInSeconds);
    }
}
