using FitnessTracking.Domain.Entity;
using FitnessTracking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class ExerciseRepository : IExerciseRepository
{
    private readonly AppDbContext _dbContext;

    public ExerciseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Exercise>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Exercise>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Exercise>().AddAsync(exercise, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<Exercise>().Update(exercise);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Exercise exercise, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<Exercise>().Remove(exercise);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Exercise>()
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
}