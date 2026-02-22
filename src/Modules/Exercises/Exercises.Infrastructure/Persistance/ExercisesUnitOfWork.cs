using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Exercises.Domain.Repositories;

namespace Exercises.Infrastructure.Persistance
{
    public sealed class ExercisesUnitOfWork(ExercisesDbContext context, IDomainEventDispatcher domainEventDispatcher) : UnitOfWork<ExercisesDbContext>(context, domainEventDispatcher), IExercisesUnitOfWork;
}