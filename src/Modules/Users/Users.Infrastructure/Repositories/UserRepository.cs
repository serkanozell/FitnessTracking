using Users.Domain.Entity;
using Users.Domain.Repositories;
using Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _dbContext;

        public UserRepository(UsersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _dbContext.Users.Include(u => u.UserRoles)
                                                                                                                               .ThenInclude(ur => ur.Role)
                                                                                                                               .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => await _dbContext.Users.Include(u => u.UserRoles)
                                                                                                                                       .ThenInclude(ur => ur.Role)
                                                                                                                                       .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        public async Task AddAsync(User user, CancellationToken cancellationToken = default) => await _dbContext.Users.AddAsync(user, cancellationToken);

        public void Update(User user) => _dbContext.Users.Update(user);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) => await _dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
    }
}