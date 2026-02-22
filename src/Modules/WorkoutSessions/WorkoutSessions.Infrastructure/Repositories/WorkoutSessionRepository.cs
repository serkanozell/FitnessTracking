using Microsoft.EntityFrameworkCore;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Infrastructure.Persistance;

namespace WorkoutSessions.Infrastructure.Repositories
{
    public class WorkoutSessionRepository : IWorkoutSessionRepository
    {
        private readonly WorkoutSessionsDbContext _context;

        public WorkoutSessionRepository(WorkoutSessionsDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<WorkoutSession>> GetListByProgramAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutSessions.Include(x => x.SessionExercises)
                .AsNoTracking()
                .Where(x => x.WorkoutProgramId == id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<WorkoutSession>> GetAllAsync(
        CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutSessions.Include(x => x.SessionExercises)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSessions.Include(x => x.SessionExercises)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public Task<List<WorkoutSession>> GetByProgramIdAsync(Guid programId, CancellationToken cancellationToken = default)
        {
            return _context.WorkoutSessions.Include(x => x.SessionExercises)
                .AsNoTracking()
                .Where(s => s.WorkoutProgramId == programId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default)
        {
            await _context.WorkoutSessions.AddAsync(session, cancellationToken);
        }

        public Task UpdateAsync(WorkoutSession session, CancellationToken cancellationToken = default)
        {
            _context.WorkoutSessions.Update(session);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.WorkoutSessions.FindAsync([id], cancellationToken);
            if (entity is null)
                return;

            _context.WorkoutSessions.Remove(entity);
        }
    }
}