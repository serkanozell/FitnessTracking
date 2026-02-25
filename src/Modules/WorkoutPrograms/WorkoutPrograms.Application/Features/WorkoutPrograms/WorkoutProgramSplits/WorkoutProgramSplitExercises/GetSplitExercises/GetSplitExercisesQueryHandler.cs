using Exercises.Contracts;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises
{
    internal sealed class GetSplitExercisesQueryHandler(IWorkoutProgramRepository _workoutProgramRepository, IExerciseModule _exerciseModule) : IQueryHandler<GetSplitExercisesQuery, Result<IReadOnlyList<WorkoutProgramSplitExerciseDto>>>
    {
        public async Task<Result<IReadOnlyList<WorkoutProgramSplitExerciseDto>>> Handle(GetSplitExercisesQuery request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdWithExercisesAsync(request.WorkoutProgramId, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var split = program.Splits.FirstOrDefault(s => s.Id == request.WorkoutSplitId);

            if (split is null)
                return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.WorkoutSplitId);

            // Tüm exercise’ları bir kere çek
            var allExercises = await _exerciseModule.GetExercisesAsync(cancellationToken);

            return split.Exercises
                .Select(e => new WorkoutProgramSplitExerciseDto
                {
                    WorkoutProgramExerciseId = e.Id,
                    ExerciseId = e.ExerciseId,
                    ExerciseName = allExercises.FirstOrDefault(ex => ex.Id == e.ExerciseId)?.Name ?? string.Empty,
                    Sets = e.Sets,
                    MinimumReps = e.MinimumReps,
                    MaximumReps = e.MaximumReps,
                    IsActive = e.IsActive,
                    IsDeleted = e.IsDeleted,
                    CreatedDate = e.CreatedDate,
                    CreatedBy = e.CreatedBy,
                    UpdatedDate = e.UpdatedDate,
                    UpdatedBy = e.UpdatedBy
                })
                .ToList();
        }
    }
}