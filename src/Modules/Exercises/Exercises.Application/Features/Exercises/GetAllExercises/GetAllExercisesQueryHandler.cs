using BuildingBlocks.Application.Pagination;
using Exercises.Application.Dtos;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    internal sealed class GetAllExercisesQueryHandler(IExerciseRepository _exerciseRepository) : IQueryHandler<GetAllExercisesQuery, Result<PagedResult<ExerciseDto>>>
    {
        public async Task<Result<PagedResult<ExerciseDto>>> Handle(GetAllExercisesQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _exerciseRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

            var dtos = items
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

            return PagedResult<ExerciseDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}