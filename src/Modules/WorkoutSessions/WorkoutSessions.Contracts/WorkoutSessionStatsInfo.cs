namespace WorkoutSessions.Contracts
{
    public record WorkoutSessionStatsInfo(int SessionCount,
                                          int TotalSets,
                                          int TotalReps,
                                          int StreakDays);
}