using System.Linq.Expressions;
using Exercises.Domain.Entity;

namespace Exercises.Domain.Repositories
{
    public interface IExerciseRepository
    {
        Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Exercise?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<TResult> Items, int TotalCount)> GetPagedAsync<TResult>(int pageNumber, int pageSize, Expression<Func<Exercise, TResult>> selector, CancellationToken cancellationToken = default);
        Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default);
        void Update(Exercise exercise);
        void Delete(Exercise exercise);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}