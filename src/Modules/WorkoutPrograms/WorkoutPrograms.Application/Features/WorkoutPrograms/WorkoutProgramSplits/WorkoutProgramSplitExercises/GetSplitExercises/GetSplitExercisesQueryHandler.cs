using BuildingBlocks.Application.Abstractions;
using Exercises.Contracts;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises
{
    internal sealed class GetSplitExercisesQueryHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IExerciseModule _exerciseModule,
        ICurrentUser _currentUser) : IQueryHandler<GetSplitExercisesQuery, Result<IReadOnlyList<WorkoutProgramSplitExerciseDto>>>
    {
        public async Task<Result<IReadOnlyList<WorkoutProgramSplitExerciseDto>>> Handle(GetSplitExercisesQuery request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdWithExercisesAsync(request.WorkoutProgramId, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, program.UserId);
            if (ownershipError is not null)
                return ownershipError;

            var split = program.Splits.FirstOrDefault(s => s.Id == request.WorkoutSplitId);

            if (split is null)
                return WorkoutProgramErrors.SplitNotFound(request.WorkoutProgramId, request.WorkoutSplitId);

            // Tüm exercise’ları bir kere çek
            var allExercises = await _exerciseModule.GetExercisesAsync(cancellationToken);

            return split.Exercises.Select(e => WorkoutProgramSplitExerciseDto.FromEntity(e, allExercises.FirstOrDefault(ex => ex.Id == e.ExerciseId)?.Name
                                                                                            ?? string.Empty))
                                  .ToList();
        }
    }
}