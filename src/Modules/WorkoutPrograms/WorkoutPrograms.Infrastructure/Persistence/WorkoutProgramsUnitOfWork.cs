using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Persistence;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Infrastructure.Persistence
{
    public sealed class WorkoutProgramsUnitOfWork(WorkoutProgramsDbContext context, IDomainEventDispatcher domainEventDispatcher) : UnitOfWork<WorkoutProgramsDbContext>(context, domainEventDispatcher), IWorkoutProgramsUnitOfWork;
}
