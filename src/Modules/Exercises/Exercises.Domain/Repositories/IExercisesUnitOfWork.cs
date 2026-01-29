namespace Exercises.Domain.Repositories
{
    public interface IExercisesUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}