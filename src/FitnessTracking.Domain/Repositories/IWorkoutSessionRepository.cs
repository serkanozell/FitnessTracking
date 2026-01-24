using FitnessTracking.Domain.Entity;

namespace FitnessTracking.Domain.Repositories
{
    public interface IWorkoutSessionRepository
    {
        Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WorkoutSession>> GetListByProgramAsync(Guid workoutProgramId, CancellationToken cancellationToken = default);
        Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default);
        Task UpdateAsync(WorkoutSession session, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}