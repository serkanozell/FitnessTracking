using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetExerciseByIdQueryHandler
    : IRequestHandler<GetExerciseByIdQuery, ExerciseDto?>
{
    private readonly IExerciseRepository _exerciseRepository;

    public GetExerciseByIdQueryHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<ExerciseDto?> Handle(
        GetExerciseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
        {
            return null;
        }

        return new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            MuscleGroup = exercise.MuscleGroup,
            Description = exercise.Description
        };
    }
}