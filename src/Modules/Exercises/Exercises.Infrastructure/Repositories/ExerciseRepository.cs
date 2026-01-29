using Exercises.Domain.Entity;
using Exercises.Domain.Repositories;
using Exercises.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default) => await _dbContext.Exercises.AsNoTracking().ToListAsync(cancellationToken);

        public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default) => await _dbContext.Exercises.AddAsync(exercise, cancellationToken);

        public async Task UpdateAsync(Exercise exercise, CancellationToken cancellationToken = default) => _dbContext.Exercises.Update(exercise);

        public async Task DeleteAsync(Exercise exercise, CancellationToken cancellationToken = default) => _dbContext.Exercises.Remove(exercise);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => await _dbContext.Exercises.AnyAsync(x => x.Id == id, cancellationToken);
    }
}