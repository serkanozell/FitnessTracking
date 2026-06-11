namespace BuildingBlocks.Web
{
    /// <summary>
    /// Named rate-limiting policy identifiers. Shared between the API host (which registers the
    /// policies) and feature endpoints (which opt in via <c>RequireRateLimiting</c>) so the policy
    /// names stay in one place instead of being duplicated as magic strings.
    /// </summary>
    public static class RateLimitPolicies
    {
        /// <summary>Strict per-IP policy guarding authentication endpoints against brute-force.</summary>
        public const string Authentication = "auth";

        /// <summary>Generous per-user policy for dashboard/analytics endpoints (high fan-out).</summary>
        public const string Dashboard = "dashboard";
    }
}
