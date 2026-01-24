using MediatR;

namespace FitnessTracking.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    public sealed class DeleteWorkoutProgramCommand : IRequest<Unit>
    {
        public Guid Id { get; init; }
    }
}