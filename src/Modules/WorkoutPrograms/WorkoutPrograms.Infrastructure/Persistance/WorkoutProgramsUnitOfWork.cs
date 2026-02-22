using BuildingBlocks.Infrastructure.Persistence;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Infrastructure.Persistance
{
    public sealed class WorkoutProgramsUnitOfWork(WorkoutProgramsDbContext context, IDomainEventDispatcher domainEventDispatcher) : UnitOfWork<WorkoutProgramsDbContext>(context, domainEventDispatcher), IWorkoutProgramsUnitOfWork;
}
