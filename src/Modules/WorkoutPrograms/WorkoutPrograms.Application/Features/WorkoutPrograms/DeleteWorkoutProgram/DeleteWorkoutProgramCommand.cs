namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    public sealed class DeleteWorkoutProgramCommand : ICommand<bool>
    {
        public Guid Id { get; init; }
    }
}