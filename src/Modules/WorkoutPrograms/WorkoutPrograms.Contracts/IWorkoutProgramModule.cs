namespace WorkoutPrograms.Contracts
{
    public interface IWorkoutProgramModule
    {
        Task<bool> ExistsAsync(Guid workoutProgramId, CancellationToken cancellationToken = default);
        Task<bool> IsOwnedByUserAsync(Guid workoutProgramId, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> ContainsExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default);
        Task<ProgramExerciseInfo?> GetProgramExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default);
        Task<ActiveProgramInfo?> GetActiveProgramByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> SplitBelongsToProgramAsync(Guid workoutProgramId, Guid workoutProgramSplitId, CancellationToken cancellationToken = default);
        Task<ProgramExerciseInfo?> GetSplitExerciseAsync(Guid workoutProgramId, Guid workoutProgramSplitId, Guid exerciseId, CancellationToken cancellationToken = default);
        Task<ProgramDetailInfo?> GetProgramWithSplitsAsync(Guid workoutProgramId, CancellationToken cancellationToken = default);
    }
}