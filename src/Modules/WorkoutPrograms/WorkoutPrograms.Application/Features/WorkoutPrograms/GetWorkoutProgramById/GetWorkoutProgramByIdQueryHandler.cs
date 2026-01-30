using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById
{
    internal sealed class GetWorkoutProgramByIdQueryHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _workoutProgramsUnitOfWork) : IRequestHandler<GetWorkoutProgramByIdQuery, WorkoutProgramDto>
    {
        public async Task<WorkoutProgramDto> Handle(GetWorkoutProgramByIdQuery request, CancellationToken cancellationToken)
        {
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.Id, cancellationToken);
            if (workoutProgram is null)
            {
                return new();
            }

            return WorkoutProgramDto.FromEntity(workoutProgram);
        }
    }
}