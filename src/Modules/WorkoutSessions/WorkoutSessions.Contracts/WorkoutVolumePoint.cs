namespace WorkoutSessions.Contracts
{
    public record WorkoutVolumePoint(DateTime Date,
                                     decimal TotalVolume,
                                     int SessionCount,
                                     int TotalSets,
                                     int TotalReps);
}
