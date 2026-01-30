using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Infrastructure.Persistance
{
    public sealed class WorkoutProgramsUnitOfWork(WorkoutProgramsDbContext _context, IDomainEventDispatcher _domainEventDispatcher) : IWorkoutProgramsUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context.SaveChangesAsync(cancellationToken);

            await _domainEventDispatcher.DispatchDomainEvents(_context);

            return result;
        }
    }
}