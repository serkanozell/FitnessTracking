namespace WorkoutPrograms.Contracts;

public interface IWorkoutProgramModule
{
    Task<bool> ExistsAsync(Guid workoutProgramId, CancellationToken cancellationToken = default);
    Task<bool> ContainsExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default);
    Task<ProgramExerciseInfo?> GetProgramExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default);
}