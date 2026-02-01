using WorkoutPrograms.Domain.Entity;

namespace WorkoutPrograms.Application.Services
{
    public interface IWorkoutProgramReadService
    {
        public Task<WorkoutProgram> GetWorkoutProgramByIdAsync(Guid workoutProgramId, CancellationToken cancellationToken = default);
        public Task<bool> ExistsAsync(Guid workoutProgramId, CancellationToken cancellationToken = default);
        public Task<bool> ContainsExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default);
    }
}