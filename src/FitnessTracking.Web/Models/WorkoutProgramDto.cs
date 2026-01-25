namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutProgramDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}