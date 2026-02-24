using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Web.Models
{
    public sealed class ExerciseEditModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PrimaryMuscleGroup { get; set; } = string.Empty;

        public string? SecondaryMuscleGroup { get; set; }

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
    }
}