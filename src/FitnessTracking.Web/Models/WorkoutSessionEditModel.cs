namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutSessionEditModel
    {
        public Guid WorkoutProgramId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
    }
}