using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits
{
    internal sealed class GetWorkoutProgramSplitsQueryHandler(IWorkoutProgramRepository _workoutProgramRepository) : IQueryHandler<GetWorkoutProgramSplitsQuery, IReadOnlyList<WorkoutProgramSplitDto>>
    {
        public async Task<IReadOnlyList<WorkoutProgramSplitDto>> Handle(GetWorkoutProgramSplitsQuery request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository
                .GetByIdAsync(request.WorkoutProgramId, cancellationToken)
                ?? throw new KeyNotFoundException(
                    $"WorkoutProgram ({request.WorkoutProgramId}) not found.");

            return workoutProgram.Splits
                          .OrderBy(x => x.Order)
                          .Select(x => new WorkoutProgramSplitDto
                          {
                              Id = x.Id,
                              WorkoutProgramId = x.WorkoutProgramId,
                              Name = x.Name,
                              Order = x.Order
                          })
                          .ToList();
        }
    }
}