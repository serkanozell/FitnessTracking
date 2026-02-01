namespace WorkoutSessions.Domain.Repositories
{
    public interface IWorkoutSessionsUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}