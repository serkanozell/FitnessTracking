using FitnessTracking.Web.Models;

public interface IWorkoutProgramsService
{
    Task<IReadOnlyList<WorkoutProgramDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkoutProgramDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateWorkoutProgramRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateWorkoutProgramRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // NEW: Splits
    Task<IReadOnlyList<WorkoutProgramSplitDto>> GetSplitsAsync(Guid programId, CancellationToken cancellationToken = default);

    Task<Guid> AddSplitAsync(Guid programId, AddSplitRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateSplitAsync(Guid programId, Guid splitId, UpdateSplitRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteSplitAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default);

    // NEW: Split exercises
    Task<IReadOnlyList<WorkoutProgramExerciseDto>> GetSplitExercisesAsync(Guid programId, Guid splitId, CancellationToken cancellationToken = default);

    Task<Guid> AddExerciseToSplitAsync(Guid programId, Guid splitId, AddProgramExerciseRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateExerciseInSplitAsync(Guid programId, Guid splitId, Guid workoutProgramExerciseId, UpdateProgramExerciseRequest request, CancellationToken cancellationToken = default);

    Task<bool> RemoveExerciseFromSplitAsync(Guid programId, Guid splitId, Guid workoutProgramExerciseId, CancellationToken cancellationToken = default);
}