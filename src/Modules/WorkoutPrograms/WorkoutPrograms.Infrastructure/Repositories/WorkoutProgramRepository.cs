using BuildingBlocks.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using WorkoutPrograms.Infrastructure.Persistance;

namespace WorkoutPrograms.Infrastructure.Repositories
{
    public class WorkoutProgramRepository : IWorkoutProgramRepository
    {
        private readonly WorkoutProgramsDbContext _context;

        public WorkoutProgramRepository(WorkoutProgramsDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<WorkoutProgram>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPrograms.Include(x => x.Splits)
                                                 .ThenInclude(x => x.Exercises)
                                                 .ToListAsync();
        }

        public async Task<(IReadOnlyList<WorkoutProgram> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPrograms.Include(x => x.Splits)
                                                 .ThenInclude(x => x.Exercises)
                                                 .OrderByDescending(x => x.CreatedDate)
                                                 .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
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
        }

        public Task UpdateAsync(WorkoutProgram program, CancellationToken cancellationToken = default)
        {
            _context.WorkoutPrograms.Update(program);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.WorkoutPrograms.FindAsync([id], cancellationToken);
            if (entity is null)
                return;

            _context.WorkoutPrograms.Remove(entity);
        }
    }
}