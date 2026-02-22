using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Persistence;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Infrastructure.Persistance
{
    public sealed class WorkoutSessionsUnitOfWork(WorkoutSessionsDbContext context, IDomainEventDispatcher domainEventDispatcher)
        : UnitOfWork<WorkoutSessionsDbContext>(context, domainEventDispatcher), IWorkoutSessionsUnitOfWork;
}