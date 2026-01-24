using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetAllExercisesQueryHandler
    : IRequestHandler<GetAllExercisesQuery, IReadOnlyList<ExerciseDto>>
{
    private readonly IExerciseRepository _exerciseRepository;

    public GetAllExercisesQueryHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<IReadOnlyList<ExerciseDto>> Handle(
        GetAllExercisesQuery request,
        CancellationToken cancellationToken)
    {
        var exercises = await _exerciseRepository
            .GetAllAsync(cancellationToken);

        return exercises
            .Select(e => new ExerciseDto
            {
                Id = e.Id,
                Name = e.Name,
                MuscleGroup = e.MuscleGroup,
                Description = e.Description
            })
            .ToList();
    }
}