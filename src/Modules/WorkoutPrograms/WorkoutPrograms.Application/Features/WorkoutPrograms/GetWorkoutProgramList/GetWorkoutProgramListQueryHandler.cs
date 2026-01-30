using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    internal sealed class GetWorkoutProgramListQueryHandler(IWorkoutProgramRepository _workoutProgramRepository) : IRequestHandler<GetWorkoutProgramListQuery, IReadOnlyList<WorkoutProgramDto>>
    {
        public async Task<IReadOnlyList<WorkoutProgramDto>> Handle(GetWorkoutProgramListQuery request, CancellationToken cancellationToken)
        {
            var workoutPrograms = await _workoutProgramRepository.GetListAsync(cancellationToken);

            return workoutPrograms.Select(WorkoutProgramDto.FromEntity).ToList();
        }
    }
}