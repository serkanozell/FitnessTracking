namespace WorkoutSessions.Contracts
{
    public interface IWorkoutSessionModule
    {
        Task<WorkoutSessionStatsInfo> GetStatsByUserAsync(Guid userId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkoutVolumePoint>> GetVolumeTrendAsync(Guid userId,
                                                                    DateTime dateFrom,
                                                                    DateTime dateTo,
                                                                    GroupingPeriod period,
                                                                    CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ExerciseProgressPoint>> GetExerciseProgressAsync(Guid userId,
                                                                            Guid exerciseId,
                                                                            DateTime dateFrom,
                                                                            DateTime dateTo,
                                                                            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ExerciseVolumeInfo>> GetExerciseVolumeBreakdownAsync(Guid userId,
                                                                               DateTime dateFrom,
                                                                               DateTime dateTo,
                                                                               CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PersonalRecordInfo>> GetPersonalRecordsAsync(Guid userId,
                                                                       int top = 10,
                                                                       CancellationToken cancellationToken = default);
    }
}