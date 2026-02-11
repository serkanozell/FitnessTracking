using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetExerciseById
{
    public sealed record GetExerciseByIdQuery(Guid Id) : IQuery<Result<ExerciseDto>>;
}