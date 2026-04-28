namespace WorkoutSessions.Contracts
{
    public record PersonalRecordInfo(Guid ExerciseId,
                                     decimal MaxWeight,
                                     int RepsAtMaxWeight,
                                     decimal Estimated1Rm,
                                     DateTime AchievedOn);
}
