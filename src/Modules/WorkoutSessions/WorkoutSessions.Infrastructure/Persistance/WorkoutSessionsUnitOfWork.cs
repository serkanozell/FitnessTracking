using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Infrastructure.Persistance
{
    public sealed class WorkoutSessionsUnitOfWork(WorkoutSessionsDbContext _context, IDomainEventDispatcher _domainEventDispatcher) : IWorkoutSessionsUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context.SaveChangesAsync(cancellationToken);

            await _domainEventDispatcher.DispatchDomainEvents(_context);

            return result;
        }
    }
}