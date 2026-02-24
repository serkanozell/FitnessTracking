using WorkoutPrograms.Contracts;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Infrastructure.Services;

public class WorkoutProgramModuleService(IWorkoutProgramRepository _workoutProgramRepository) : IWorkoutProgramModule
{
    public async Task<bool> ExistsAsync(Guid workoutProgramId, CancellationToken cancellationToken = default)
    {
        var result = await _workoutProgramRepository.GetByIdAsync(workoutProgramId, cancellationToken);
        return result is not null;
    }

    public async Task<bool> ContainsExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default)
    {
        var result = await _workoutProgramRepository.GetByIdWithExercisesAsync(workoutProgramId, cancellationToken);

        if (result is null)
            return false;

        return result.ContainsExercise(exerciseId);
    }

    public async Task<ProgramExerciseInfo?> GetProgramExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default)
    {
        var result = await _workoutProgramRepository.GetByIdWithExercisesAsync(workoutProgramId, cancellationToken);

        if (result is null)
            return null;

        var exercise = result.Splits
                             .SelectMany(s => s.Exercises)
                             .FirstOrDefault(x => x.ExerciseId == exerciseId);

        if (exercise is null)
            return null;

        return new ProgramExerciseInfo(exercise.ExerciseId, exercise.Sets);
    }
}
