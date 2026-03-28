namespace BuildingBlocks.Infrastructure.Persistence.Caching
{
    public static class CacheKeys
    {
        // Exercises (global — no user scope, safe to cache)
        public static string Exercises() => "exercises:all";
        public static string ExerciseById(Guid id) => $"exercises:{id}";
    }
}