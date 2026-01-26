using FitnessTracking.Domain.Repositories;
using MediatR;

internal sealed class GetWorkoutProgramExercisesQueryHandler
    : IRequestHandler<GetWorkoutProgramExercisesQuery, IReadOnlyList<WorkoutProgramExerciseDto>>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;
    private readonly IExerciseRepository _exerciseRepository;

    public GetWorkoutProgramExercisesQueryHandler(IWorkoutProgramRepository workoutProgramRepository, IExerciseRepository exerciseRepository)
    {
        _workoutProgramRepository = workoutProgramRepository;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<IReadOnlyList<WorkoutProgramExerciseDto>> Handle(
        GetWorkoutProgramExercisesQuery request,
        CancellationToken cancellationToken)
    {
        // Repository imzan bu şekilde varsayıldı; gerekirse uyarlarsın.
        var program = await _workoutProgramRepository.GetByIdWithExercisesAsync(
            request.WorkoutProgramId,
            cancellationToken);

        if (program is null || program.WorkoutProgramExercises is null)
        {
            return Array.Empty<WorkoutProgramExerciseDto>();
        }

        // Tüm exercise’ları bir kere çek
        var allExercises = await _exerciseRepository.GetAllAsync(cancellationToken);
        var exerciseLookup = allExercises.ToDictionary(x => x.Id, x => x.Name);

        var result = program.WorkoutProgramExercises
            .Select(x => new WorkoutProgramExerciseDto
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