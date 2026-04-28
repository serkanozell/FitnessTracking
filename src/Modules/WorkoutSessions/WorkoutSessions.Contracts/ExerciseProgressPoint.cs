namespace WorkoutSessions.Contracts
{
    public record ExerciseProgressPoint(DateTime Date,
                                        decimal MaxWeight,
                                        int MaxReps,
                                        decimal TotalVolume,
                                        decimal Estimated1Rm);
}
