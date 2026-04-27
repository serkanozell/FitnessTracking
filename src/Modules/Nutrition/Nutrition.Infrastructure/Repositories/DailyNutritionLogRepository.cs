using BuildingBlocks.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entity;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence;

namespace Nutrition.Infrastructure.Repositories
{
    public class DailyNutritionLogRepository(NutritionDbContext _context) : IDailyNutritionLogRepository
    {
        public async Task<DailyNutritionLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.DailyNutritionLogs
                .Include(x => x.Entries)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<DailyNutritionLog?> GetByUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.DailyNutritionLogs
                .Include(x => x.Entries)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date.Date, cancellationToken);
        }

        public async Task<(IReadOnlyList<DailyNutritionLog> Items, int TotalCount)> GetPagedByUserAsync(
            Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.DailyNutritionLogs
                .Include(x => x.Entries)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Date);

            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task AddAsync(DailyNutritionLog log, CancellationToken cancellationToken = default)
            => await _context.DailyNutritionLogs.AddAsync(log, cancellationToken);

        public void Update(DailyNutritionLog log)
            => _context.DailyNutritionLogs.Update(log);
    }
}
