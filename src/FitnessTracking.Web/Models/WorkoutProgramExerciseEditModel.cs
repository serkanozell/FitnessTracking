using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutProgramExerciseEditModel
    {
        [Required]
        public Guid? ExerciseId { get; set; }

        [Range(1, 100)]
        public int Sets { get; set; } = 3;

        [Range(1, 100)]
        public int TargetReps { get; set; } = 10;
    }
}