namespace Exercises.Application.Caching
{
    // Single source of truth for Exercises cache keys. Both cacheable queries and
    // cache-invalidating commands MUST derive their keys/prefixes from here so the
    // read and invalidation sides can never silently drift apart.
    public static class ExerciseCacheKeys
    {
        // Prefix covering every paginated "all exercises" entry. Used for invalidation.
        public const string AllPrefix = "exercises:all";

        public static string All(int pageNumber, int pageSize)
            => $"{AllPrefix}:p{pageNumber}:s{pageSize}";

        public static string ById(Guid id)
            => $"exercises:{id}";
    }
}
