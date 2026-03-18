using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Mvc.Models;

public sealed class WorkoutProgramExerciseEditModel
{
    [Required]
    public Guid? ExerciseId { get; set; }

    [Range(1, 100)]
    public int Sets { get; set; } = 3;

    [Range(1, 100)]
    public int MinimumReps { get; set; } = 8;

    [Range(1, 100)]
    public int MaximumReps { get; set; } = 12;
}
