using System.ComponentModel.DataAnnotations;

namespace FitnessTracking.Mvc.Models;

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

    public static string[] MuscleGroups =>
    [
        "Chest", "Back", "Shoulders", "Biceps", "Triceps",
        "Quadriceps", "Hamstrings", "Glutes", "Calves",
        "Core", "Forearms", "Traps", "Lats", "FullBody"
    ];
}
