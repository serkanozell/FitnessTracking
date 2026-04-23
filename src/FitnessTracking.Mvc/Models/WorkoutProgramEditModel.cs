using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Mvc.Models;

public sealed class WorkoutProgramEditModel
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Today;
}
