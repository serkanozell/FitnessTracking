using FitnessTracking.Domain.Entity;
using FitnessTracking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class WorkoutProgramRepository : IWorkoutProgramRepository
{
    private readonly AppDbContext _context;

    public WorkoutProgramRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WorkoutProgram>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WorkoutPrograms.Include(x => x.Splits)
                                             .ThenInclude(x => x.Exercises)
                                             .ToListAsync();
    }

    public async Task<WorkoutProgram?> GetByIdWithExercisesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkoutPrograms.Include(x => x.Splits)
                                             .ThenInclude(x => x.Exercises)
                                             .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<WorkoutProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.WorkoutPrograms.Include(x => x.Splits)
                                       .ThenInclude(x => x.Exercises)
                                       .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(WorkoutProgram program, CancellationToken cancellationToken = default)
    {
        await _context.WorkoutPrograms.AddAsync(program, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkoutProgram program, CancellationToken cancellationToken = default)
    {
        _context.WorkoutPrograms.Update(program);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkoutPrograms.FindAsync([id], cancellationToken);
        if (entity is null)
            return;

        _context.WorkoutPrograms.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}