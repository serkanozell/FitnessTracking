namespace FitnessTracking.Web.Models
{
    public sealed class ExerciseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string MuscleGroup { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }
}