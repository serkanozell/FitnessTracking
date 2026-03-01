using BuildingBlocks.Infrastructure.Persistence;
using Exercises.Domain.Repositories;

namespace Exercises.Infrastructure.Persistence
{
    public sealed class ExercisesUnitOfWork(ExercisesDbContext context) : UnitOfWork<ExercisesDbContext>(context), IExercisesUnitOfWork;
}