using FitnessTracking.Client.Services.WorkoutPrograms.Models;

namespace FitnessTracking.Client.Services.WorkoutPrograms
{
    public interface IWorkoutProgramsService
    {
        Task<IReadOnlyList<WorkoutProgramDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<WorkoutProgramDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkoutProgramExerciseDto>> GetProgramExercisesAsync(
            Guid programId,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(CreateWorkoutProgramRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, UpdateWorkoutProgramRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Guid> AddExerciseAsync(
            Guid programId,
            AddProgramExerciseRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateExerciseAsync(
            Guid programId,
            Guid workoutProgramExerciseId,
            UpdateProgramExerciseRequest request,
            CancellationToken cancellationToken = default);

        Task RemoveExerciseAsync(
            Guid programId,
            Guid workoutProgramExerciseId,
            CancellationToken cancellationToken = default);
    }
}