using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutProgramEditModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime EndDate { get; set; } = DateTime.Today;
    }
}