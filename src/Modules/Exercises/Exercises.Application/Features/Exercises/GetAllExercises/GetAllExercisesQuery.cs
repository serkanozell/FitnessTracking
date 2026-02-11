using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    public sealed record GetAllExercisesQuery : IQuery<Result<IReadOnlyList<ExerciseDto>>>;
}