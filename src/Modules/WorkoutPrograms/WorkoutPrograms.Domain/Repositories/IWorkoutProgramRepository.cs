using WorkoutPrograms.Domain.Entity;

namespace WorkoutPrograms.Domain.Repositories
{
    public interface IWorkoutProgramRepository
    {
        Task<WorkoutProgram?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WorkoutProgram>> GetListAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<WorkoutProgram> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<WorkoutProgram?> GetByIdWithExercisesAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(WorkoutProgram program, CancellationToken cancellationToken = default);
        Task UpdateAsync(WorkoutProgram program, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}