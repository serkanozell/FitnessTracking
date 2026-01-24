using FitnessTracking.Domain.Entity;

namespace FitnessTracking.Domain.Repositories
{
    public interface IWorkoutProgramRepository
    {
        Task<WorkoutProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WorkoutProgram>> GetListAsync(CancellationToken cancellationToken = default);
        Task AddAsync(WorkoutProgram program, CancellationToken cancellationToken = default);
        Task UpdateAsync(WorkoutProgram program, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}