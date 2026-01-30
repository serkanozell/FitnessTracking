using Exercises.Application.Dtos;
using Exercises.Application.Features.Exercises.GetExerciseById;

internal sealed class GetExerciseByIdQueryHandler(IExerciseRepository _exerciseRepository) : IQueryHandler<GetExerciseByIdQuery, ExerciseDto>
{
    public async Task<ExerciseDto> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
        {
            return new();
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