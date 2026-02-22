namespace BuildingBlocks.Infrastructure.Persistence.Caching
{
    public static class CacheKeys
    {
        // Exercises
        public static string Exercises() => "exercises:all";
        public static string ExerciseById(Guid id) => $"exercises:{id}";

        // Workout Programs
        public static string WorkoutPrograms() => "workoutprograms:all";
        public static string WorkoutProgramById(Guid id) => $"workoutprograms:{id}";
        public static string WorkoutProgramSplits(Guid programId) => $"workoutprograms:{programId}:splits";
        public static string SplitExercises(Guid programId, Guid splitId) => $"workoutprograms:{programId}:splits:{splitId}:exercises";

        // Workout Sessions
        public static string WorkoutSessions() => "workoutsessions:all";
        public static string WorkoutSessionById(Guid id) => $"workoutsessions:{id}";
        public static string WorkoutSessionsByProgram(Guid programId) => $"workoutsessions:program:{programId}";
        public static string SessionExercises(Guid sessionId) => $"workoutsessions:{sessionId}:exercises";
    }
}