namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutSessionDto
    {
        public Guid Id { get; set; }
        public Guid WorkoutProgramId { get; set; }
        public DateTime Date { get; set; }
    }
}