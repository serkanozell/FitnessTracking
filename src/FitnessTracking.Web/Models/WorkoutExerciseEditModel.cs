namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutExerciseEditModel
    {
        public Guid ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public decimal Weight { get; set; }
        public int Reps { get; set; }
    }
}