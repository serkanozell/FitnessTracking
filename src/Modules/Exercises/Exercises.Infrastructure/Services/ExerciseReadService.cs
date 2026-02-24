using Exercises.Contracts;
using Exercises.Domain.Repositories;

namespace Exercises.Infrastructure.Services
{
    internal sealed class ExerciseModuleService(IExerciseRepository _exerciseRepository) : IExerciseModule
    {
        public async Task<IReadOnlyCollection<ExerciseInfo>> GetExercisesAsync(CancellationToken cancellationToken = default)
        {
            var result = await _exerciseRepository.GetAllAsync(cancellationToken);

            return result.Select(x => new ExerciseInfo(x.Id,
                                                       x.Name,
                                                       x.PrimaryMuscleGroup.ToString(),
                                                       x.SecondaryMuscleGroup?.ToString(),
                                                       x.Description))
                         .ToList();
        }
    }
}