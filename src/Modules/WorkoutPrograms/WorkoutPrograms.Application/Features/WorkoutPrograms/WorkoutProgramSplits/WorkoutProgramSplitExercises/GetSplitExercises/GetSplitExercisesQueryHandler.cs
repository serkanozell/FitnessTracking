using Exercises.Application.Services;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises
{
    internal sealed class GetSplitExercisesQueryHandler(IWorkoutProgramRepository _workoutProgramRepository, IExerciseReadService _exerciseReadService) : IQueryHandler<GetSplitExercisesQuery, IReadOnlyList<WorkoutProgramSplitExerciseDto>>
    {
        public async Task<IReadOnlyList<WorkoutProgramSplitExerciseDto>> Handle(GetSplitExercisesQuery request, CancellationToken cancellationToken)
        {
            // Repository imzan bu şekilde varsayıldı; gerekirse uyarlarsın.
            var workoutProgram = await _workoutProgramRepository.GetByIdWithExercisesAsync(request.WorkoutProgramId, cancellationToken);

            if (workoutProgram is null || workoutProgram.Splits is null)
            {
                return Array.Empty<WorkoutProgramSplitExerciseDto>();
            }

            var split = workoutProgram.Splits.FirstOrDefault(s => s.Id == request.WorkoutSplitId);

            if (split is null)
            {
                return Array.Empty<WorkoutProgramSplitExerciseDto>();
            }

            // Tüm exercise’ları bir kere çek
            // Exercise ayrı modüle taşındığı için burada farklı bir çözüm bakacağız
            var allExercises = await _exerciseReadService.GetNamesByIdsAsync(cancellationToken);

            var result = split.Exercises
                .Select(x => new WorkoutProgramSplitExerciseDto
                {
                    WorkoutProgramExerciseId = x.Id,
                    ExerciseId = x.ExerciseId,
                    ExerciseName = allExercises.TryGetValue(x.ExerciseId, out var name) ? name : string.Empty,
                    Sets = x.Sets,
                    TargetReps = x.TargetReps
                })
                .ToList()
                .AsReadOnly();

            return result;
        }
    }
}