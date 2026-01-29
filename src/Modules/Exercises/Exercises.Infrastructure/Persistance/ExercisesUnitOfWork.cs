using Exercises.Domain.Repositories;

namespace Exercises.Infrastructure.Persistance
{
    public sealed class ExercisesUnitOfWork(ExercisesDbContext _context, IDomainEventDispatcher _domainEventDispatcher) : IExercisesUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = _context.SaveChangesAsync(cancellationToken);

            await _domainEventDispatcher.DispatchDomainEvents(_context);

            return result.Result;
        }
    }
}