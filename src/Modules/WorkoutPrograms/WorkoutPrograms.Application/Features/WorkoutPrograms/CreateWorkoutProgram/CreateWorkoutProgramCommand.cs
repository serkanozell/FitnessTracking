namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    public sealed class CreateWorkoutProgramCommand : ICommand<Guid>
    {
        public string Name { get; init; } = default!;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}