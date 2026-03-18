using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface IExercisesService
{
    Task<PagedResult<ExerciseDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<ExerciseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(ExerciseEditModel model, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, ExerciseEditModel model, CancellationToken cancellationToken = default);
    Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
