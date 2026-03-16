namespace WorkoutSessions.Contracts
{
    public interface IWorkoutSessionModule
    {
        Task<WorkoutSessionStatsInfo> GetStatsByUserAsync(Guid userId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);
    }
}