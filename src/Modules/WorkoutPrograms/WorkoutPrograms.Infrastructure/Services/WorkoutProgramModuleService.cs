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

    public async Task<bool> IsOwnedByUserAsync(Guid workoutProgramId, Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _workoutProgramRepository.GetByIdAsync(workoutProgramId, cancellationToken);
        return result is not null && result.UserId == userId;
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

    public async Task<ActiveProgramInfo?> GetActiveProgramByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var program = await _workoutProgramRepository.GetActiveByUserIdAsync(userId, cancellationToken);

        var active = program.FirstOrDefault(p => p.IsActive && p.EndDate >= DateTime.Today);

        if (active is null) return null;

        return new ActiveProgramInfo(active.Id, active.Name, active.StartDate, active.EndDate);
    }

    public async Task<bool> SplitBelongsToProgramAsync(Guid workoutProgramId, Guid workoutProgramSplitId, CancellationToken cancellationToken = default)
    {
        var program = await _workoutProgramRepository.GetByIdWithExercisesAsync(workoutProgramId, cancellationToken);

        if (program is null)
            return false;

        return program.Splits.Any(s => s.Id == workoutProgramSplitId && !s.IsDeleted);
    }

    public async Task<ProgramExerciseInfo?> GetSplitExerciseAsync(Guid workoutProgramId, Guid workoutProgramSplitId, Guid exerciseId, CancellationToken cancellationToken = default)
    {
        var program = await _workoutProgramRepository.GetByIdWithExercisesAsync(workoutProgramId, cancellationToken);

        if (program is null)
            return null;

        var split = program.Splits.FirstOrDefault(s => s.Id == workoutProgramSplitId && !s.IsDeleted);

        if (split is null)
            return null;

        var exercise = split.Exercises.FirstOrDefault(x => x.ExerciseId == exerciseId);

        if (exercise is null)
            return null;

        return new ProgramExerciseInfo(exercise.ExerciseId, exercise.Sets);
    }

    public async Task<ProgramDetailInfo?> GetProgramWithSplitsAsync(Guid workoutProgramId, CancellationToken cancellationToken = default)
    {
        var program = await _workoutProgramRepository.GetByIdWithExercisesAsync(workoutProgramId, cancellationToken);

        if (program is null)
            return null;

        var splits = program.Splits
            .Select(s => new ProgramSplitSummaryInfo(
                s.Id,
                s.Name,
                s.Order,
                s.IsDeleted,
                s.Exercises
                    .Select(e => new ProgramSplitExerciseInfo(e.ExerciseId, e.IsDeleted))
                    .ToList()))
            .ToList();

        return new ProgramDetailInfo(program.Id, program.Name, splits);
    }
}