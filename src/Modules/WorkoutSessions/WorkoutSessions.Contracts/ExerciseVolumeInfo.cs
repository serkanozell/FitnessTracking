namespace WorkoutSessions.Contracts
{
    public record ExerciseVolumeInfo(Guid ExerciseId,
                                     decimal TotalVolume,
                                     int SetCount,
                                     int TotalReps);
}
