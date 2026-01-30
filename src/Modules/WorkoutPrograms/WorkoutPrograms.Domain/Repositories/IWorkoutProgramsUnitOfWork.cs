namespace WorkoutPrograms.Domain.Repositories
{
    public interface IWorkoutProgramsUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}