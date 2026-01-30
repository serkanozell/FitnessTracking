using Exercises.Application.Services;
using Exercises.Domain.Repositories;

namespace Exercises.Infrastructure.Services
{
    internal sealed class ExerciseReadService(IExerciseRepository _exerciseRepository) : IExerciseReadService
    {
        public async Task<IReadOnlyDictionary<Guid, string>> GetNamesByIdsAsync(CancellationToken cancellationToken = default)
        {
            // Repo'da filtreli method olmadığı için şimdilik GetAllAsync + filter.
            // (İyileştirme: IExerciseRepository'ye GetByIdsAsync eklemek.)
            var result = await _exerciseRepository.GetAllAsync(cancellationToken);

            return result.ToDictionary(x => x.Id, x => x.Name);
        }
    }
}