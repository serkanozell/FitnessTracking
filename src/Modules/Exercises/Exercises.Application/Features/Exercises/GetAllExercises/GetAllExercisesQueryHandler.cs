using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    internal sealed class GetAllExercisesQueryHandler(IExerciseRepository _exerciseRepository) : IQueryHandler<GetAllExercisesQuery, IReadOnlyList<ExerciseDto>>
    {
        public async Task<IReadOnlyList<ExerciseDto>> Handle(GetAllExercisesQuery request, CancellationToken cancellationToken)
        {
            var exercises = await _exerciseRepository.GetAllAsync(cancellationToken);

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
}