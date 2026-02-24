namespace Exercises.Contracts;

public interface IExerciseModule
{
    Task<IReadOnlyCollection<ExerciseInfo>> GetExercisesAsync(CancellationToken cancellationToken = default);
}
