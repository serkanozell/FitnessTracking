using WorkoutSessions.Domain.Entity;

public interface IWorkoutSessionRepository
{
    Task<WorkoutSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkoutSession>> GetListByProgramAsync(Guid workoutProgramId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkoutSession>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkoutSession session, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}