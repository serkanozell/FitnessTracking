using FitnessTracking.Domain.Repositories;
using MediatR;

internal sealed class GetWorkoutProgramSplitExercisesQueryHandler
    : IRequestHandler<GetWorkoutProgramSplitExercisesQuery, IReadOnlyList<WorkoutProgramSplitExerciseDto>>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;
    private readonly IExerciseRepository _exerciseRepository;

    public GetWorkoutProgramSplitExercisesQueryHandler(IWorkoutProgramRepository workoutProgramRepository, IExerciseRepository exerciseRepository)
    {
        _workoutProgramRepository = workoutProgramRepository;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<IReadOnlyList<WorkoutProgramSplitExerciseDto>> Handle(
        GetWorkoutProgramSplitExercisesQuery request,
        CancellationToken cancellationToken)
    {
        // Repository imzan bu şekilde varsayıldı; gerekirse uyarlarsın.
        var program = await _workoutProgramRepository.GetByIdWithExercisesAsync(
            request.WorkoutProgramId,
            cancellationToken);

        if (program is null || program.Splits is null)
        {
            return Array.Empty<WorkoutProgramSplitExerciseDto>();
        }

        var split = program.Splits.FirstOrDefault(s => s.Id == request.WorkoutSplitId);

        if (split is null)
        {
            return Array.Empty<WorkoutProgramSplitExerciseDto>();
        }

        // Tüm exercise’ları bir kere çek
        var allExercises = await _exerciseRepository.GetAllAsync(cancellationToken);
        var exerciseLookup = allExercises.ToDictionary(x => x.Id, x => x.Name);

        var result = split.Exercises
            .Select(x => new WorkoutProgramSplitExerciseDto
            {
                WorkoutProgramExerciseId = x.Id,
                ExerciseId = x.ExerciseId,
                ExerciseName = exerciseLookup.TryGetValue(x.ExerciseId, out var name)
                    ? name
                    : string.Empty,
                Sets = x.Sets,
                TargetReps = x.TargetReps
            })
            .ToList()
            .AsReadOnly();

        return result;
    }
}