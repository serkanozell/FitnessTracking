using System.Linq.Expressions;
using Users.Domain.Entity;

namespace Users.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TResult>> GetAllAsync<TResult>(Expression<Func<Role, TResult>> selector, CancellationToken cancellationToken = default);
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        void Update(Role role);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
