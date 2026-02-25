using Exercises.Domain.Entity;

namespace Exercises.Domain.Repositories
{
    public interface IExerciseRepository
    {
        Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Exercise?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Exercise> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default);
        Task UpdateAsync(Exercise exercise, CancellationToken cancellationToken = default);
        Task DeleteAsync(Exercise exercise, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}