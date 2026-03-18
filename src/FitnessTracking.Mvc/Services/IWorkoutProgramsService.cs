using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface IWorkoutProgramsService
{
    Task<PagedResult<WorkoutProgramDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<WorkoutProgramDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateWorkoutProgramRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateWorkoutProgramRequest request, CancellationToken cancellationToken = default);
    Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkoutProgramSplitDto>> GetSplitsAsync(Guid programId, CancellationToken cancellationToken = default);
    Task<Guid> AddSplitAsync(Guid programId, AddSplitRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateSplitAsync(Guid programId, Guid splitId, UpdateSplitRequest request, CancellationToken cancellationToken = default);
    Task<bool> ActivateSplitAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default);
    Task<bool> DeleteSplitAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkoutProgramExerciseDto>> GetSplitExercisesAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default);
    Task<Guid> AddExerciseToSplitAsync(Guid programId, Guid splitId, AddProgramExerciseRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateExerciseInSplitAsync(Guid programId, Guid splitId, Guid exerciseId, UpdateProgramExerciseRequest request, CancellationToken cancellationToken = default);
    Task<bool> ActivateSplitExerciseAsync(Guid programId, Guid splitId, Guid exerciseId, CancellationToken cancellationToken = default);
    Task<bool> RemoveExerciseFromSplitAsync(Guid programId, Guid splitId, Guid exerciseId, CancellationToken cancellationToken = default);
}
