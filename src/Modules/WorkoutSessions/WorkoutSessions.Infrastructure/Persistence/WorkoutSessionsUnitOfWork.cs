using BuildingBlocks.Infrastructure.Persistence;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Infrastructure.Persistence
{
    public sealed class WorkoutSessionsUnitOfWork(WorkoutSessionsDbContext context) : UnitOfWork<WorkoutSessionsDbContext>(context), IWorkoutSessionsUnitOfWork;
}