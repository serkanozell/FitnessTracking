using Exercises.Application.Dtos;
using Exercises.Application.Errors;
using Exercises.Application.Features.Exercises.GetExerciseById;

internal sealed class GetExerciseByIdQueryHandler(IExerciseRepository _exerciseRepository) : IQueryHandler<GetExerciseByIdQuery, Result<ExerciseDto>>
{
    public async Task<Result<ExerciseDto>> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
            return ExerciseErrors.NotFound(request.Id);

        return new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            MuscleGroup = exercise.MuscleGroup,
            Description = exercise.Description
        };
    }
}