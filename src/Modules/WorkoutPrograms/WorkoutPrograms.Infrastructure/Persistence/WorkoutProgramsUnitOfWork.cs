using BuildingBlocks.Infrastructure.Persistence;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Infrastructure.Persistence
{
    public sealed class WorkoutProgramsUnitOfWork(WorkoutProgramsDbContext context) : UnitOfWork<WorkoutProgramsDbContext>(context), IWorkoutProgramsUnitOfWork;
}
