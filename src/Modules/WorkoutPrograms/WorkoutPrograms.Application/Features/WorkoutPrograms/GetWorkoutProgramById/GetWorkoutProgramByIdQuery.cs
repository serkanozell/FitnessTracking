using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById
{
    public sealed class GetWorkoutProgramByIdQuery : IQuery<WorkoutProgramDto>
    {
        public Guid Id { get; init; }
    }
}