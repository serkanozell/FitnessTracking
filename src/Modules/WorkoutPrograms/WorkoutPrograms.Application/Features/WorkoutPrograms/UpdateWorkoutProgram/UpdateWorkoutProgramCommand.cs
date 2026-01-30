namespace WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram
{
    public sealed class UpdateWorkoutProgramCommand : ICommand<Unit>
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}