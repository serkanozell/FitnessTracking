using FitnessTracking.Web.Models;

namespace FitnessTracking.Web.Services
{
    public interface IWorkoutSessionsService
    {
        Task<IReadOnlyList<WorkoutSessionDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<WorkoutSessionDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            WorkoutSessionEditModel model,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            Guid id,
            WorkoutSessionEditModel model,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<WorkoutSessionDetailsDto?> GetDetailsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkoutExerciseDto>> GetWorkoutExercisesAsync(
            Guid sessionId,
            CancellationToken cancellationToken = default);

        Task<Guid> AddWorkoutExerciseAsync(
            Guid sessionId,
            WorkoutExerciseEditModel model,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateWorkoutExerciseAsync(
            Guid sessionId,
            Guid workoutExerciseId,
            WorkoutExerciseEditModel model,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteWorkoutExerciseAsync(
            Guid sessionId,
            Guid workoutExerciseId,
            CancellationToken cancellationToken = default);
    }
}