using BuildingBlocks.Infrastructure.Pagination;
using Exercises.Domain.Entity;
using Exercises.Domain.Repositories;
using Exercises.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Exercises.Infrastructure.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ExercisesDbContext _dbContext;

        public ExerciseRepository(ExercisesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _dbContext.Exercises.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<Exercise?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbContext.Exercises.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        public async Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default) => await _dbContext.Exercises.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<(IReadOnlyList<TResult> Items, int TotalCount)> GetPagedAsync<TResult>(int pageNumber, int pageSize, Expression<Func<Exercise, TResult>> selector, CancellationToken cancellationToken = default) =>
            await _dbContext.Exercises.AsNoTracking().OrderBy(x => x.Name).Select(selector).ToPagedListAsync(pageNumber, pageSize, cancellationToken);

        public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default) => await _dbContext.Exercises.AddAsync(exercise, cancellationToken);

        public void Update(Exercise exercise) => _dbContext.Exercises.Update(exercise);

        public void Delete(Exercise exercise) => _dbContext.Exercises.Remove(exercise);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => await _dbContext.Exercises.AnyAsync(x => x.Id == id, cancellationToken);
    }
}