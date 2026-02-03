namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutProgramSplitDto
    {
        public Guid Id { get; init; }
        public Guid WorkoutProgramId { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Order { get; init; }
    }
}