namespace FitnessTracking.Application.Dtos
{
    public sealed class WorkoutProgramSplitDto
    {
        public Guid Id { get; init; }
        public Guid WorkoutProgramId { get; init; }
        public string Name { get; init; } = default!;
        public int Order { get; init; }
    }
}