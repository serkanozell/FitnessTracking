using BuildingBlocks.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entity;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence;

namespace Nutrition.Infrastructure.Repositories
{
    public class FoodRepository(NutritionDbContext _context) : IFoodRepository
    {
        public async Task<Food?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.Foods.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyList<Food>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Foods
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Food>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Foods
                .Where(x => (x.UserId == null || x.UserId == userId) && !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Food> Items, int TotalCount)> GetPagedAsync(
            Guid? userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Foods
                .Where(x => x.UserId == null || x.UserId == userId)
                .OrderBy(x => x.Name);

            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task AddAsync(Food food, CancellationToken cancellationToken = default)
            => await _context.Foods.AddAsync(food, cancellationToken);

        public void Update(Food food)
            => _context.Foods.Update(food);
    }
}
