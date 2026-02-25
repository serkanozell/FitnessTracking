using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits
{
    internal sealed class GetWorkoutProgramSplitsQueryHandler(IWorkoutProgramRepository _workoutProgramRepository) : IQueryHandler<GetWorkoutProgramSplitsQuery, Result<IReadOnlyList<WorkoutProgramSplitDto>>>
    {
        public async Task<Result<IReadOnlyList<WorkoutProgramSplitDto>>> Handle(GetWorkoutProgramSplitsQuery request, CancellationToken cancellationToken)
        {
            var program = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);

            if (program is null)
                return WorkoutProgramErrors.NotFound(request.WorkoutProgramId);

            return program.Splits.Select(s =>
                                         new WorkoutProgramSplitDto
                                         {
                                             Id = s.Id,
                                             WorkoutProgramId = s.WorkoutProgramId,
                                             Name = s.Name,
                                             Order = s.Order,
                                             IsActive = s.IsActive,
                                             IsDeleted = s.IsDeleted,
                                             CreatedDate = s.CreatedDate,
                                             CreatedBy = s.CreatedBy,
                                             UpdatedDate = s.UpdatedDate,
                                             UpdatedBy = s.UpdatedBy
                                         })
                                           .OrderBy(x => x.Order)
                                           .ToList();
        }
    }
}