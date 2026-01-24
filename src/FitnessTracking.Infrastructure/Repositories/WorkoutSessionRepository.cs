using FitnessTracking.Domain.Entity;
using FitnessTracking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public class WorkoutSessionRepository : IWorkoutSessionRepository
{
    private readonly AppDbContext _context;

    public WorkoutSessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WorkoutSession>> GetListByProgramAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkoutSessions.Include(x => x.WorkoutExercises)
            .AsNoTracking()
            .Where(x => x.WorkoutProgramId == id)
            .ToListAsync(cancellationToken);
    }

    public Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.WorkoutSessions.Include(x => x.WorkoutExercises)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public Task<List<WorkoutSession>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default)
    {
        return _context.WorkoutSessions.Include(x => x.WorkoutExercises)
            .AsNoTracking()
            .Where(s => s.WorkoutProgramId == programId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default)
    {
        await _context.WorkoutSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkoutSession session, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkoutSessions.FindAsync([id], cancellationToken);
        if (entity is null)
            return;

        _context.WorkoutSessions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}