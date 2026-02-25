using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    internal sealed class GetAllExercisesQueryHandler(IExerciseRepository _exerciseRepository) : IQueryHandler<GetAllExercisesQuery, Result<IReadOnlyList<ExerciseDto>>>
    {
        public async Task<Result<IReadOnlyList<ExerciseDto>>> Handle(GetAllExercisesQuery request, CancellationToken cancellationToken)
        {
            var exercises = await _exerciseRepository.GetAllAsync(cancellationToken);

            var dtos = exercises
                .Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    PrimaryMuscleGroup = e.PrimaryMuscleGroup.ToString(),
                    SecondaryMuscleGroup = e.SecondaryMuscleGroup?.ToString(),
                    Description = e.Description,
                    IsActive = e.IsActive,
                    IsDeleted = e.IsDeleted,
                    CreatedDate = e.CreatedDate,
                    CreatedBy = e.CreatedBy,
                    UpdatedDate = e.UpdatedDate,
                    UpdatedBy = e.UpdatedBy
                })
                .ToList();

            return Result<IReadOnlyList<ExerciseDto>>.Success(dtos);
        }
    }
}