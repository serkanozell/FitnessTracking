using FitnessTracking.Web.Models;

public interface IExercisesService
{
    Task<IReadOnlyList<ExerciseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ExerciseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(ExerciseEditModel model, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, ExerciseEditModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}