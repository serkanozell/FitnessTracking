using Users.Domain.Entity;
using Users.Domain.Repositories;
using Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UsersDbContext _dbContext;

        public RoleRepository(UsersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
            await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Roles.AsNoTracking().ToListAsync(cancellationToken);

        public async Task AddAsync(Role role, CancellationToken cancellationToken = default) =>
            await _dbContext.Roles.AddAsync(role, cancellationToken);

        public void Update(Role role) =>
            _dbContext.Roles.Update(role);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _dbContext.Roles.AnyAsync(x => x.Id == id, cancellationToken);
    }
}
