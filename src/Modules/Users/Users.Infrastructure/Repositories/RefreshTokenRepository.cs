using Microsoft.EntityFrameworkCore;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Users.Infrastructure.Persistence;

namespace Users.Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository(UsersDbContext _context) : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) => await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

        public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => await _context.RefreshTokens.Where(x => x.UserId == userId
                                                                                                                                                                                     && x.RevokedOnUtc == null
                                                                                                                                                                                     && x.ExpiresOnUtc > DateTime.Now)
                                                                                                                                                                         .ToListAsync(cancellationToken);

        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default) => await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        public void Update(RefreshToken refreshToken) => _context.RefreshTokens.Update(refreshToken);
    }
}