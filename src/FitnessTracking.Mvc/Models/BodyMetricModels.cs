namespace FitnessTracking.Mvc.Models;

public sealed class BodyMetricDto
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public decimal? Weight { get; init; }
    public decimal? Height { get; init; }
    public decimal? BodyFatPercentage { get; init; }
    public decimal? MuscleMass { get; init; }
    public decimal? WaistCircumference { get; init; }
    public decimal? ChestCircumference { get; init; }
    public decimal? ArmCircumference { get; init; }
    public decimal? HipCircumference { get; init; }
    public decimal? ThighCircumference { get; init; }
    public decimal? NeckCircumference { get; init; }
    public decimal? ShoulderCircumference { get; init; }
    public string? Note { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? CreatedDate { get; init; }
}

public sealed class BodyMetricEditModel
{
    public DateTime Date { get; set; } = DateTime.Today;
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? BodyFatPercentage { get; set; }
    public decimal? MuscleMass { get; set; }
    public decimal? WaistCircumference { get; set; }
    public decimal? ChestCircumference { get; set; }
    public decimal? ArmCircumference { get; set; }
    public decimal? HipCircumference { get; set; }
    public decimal? ThighCircumference { get; set; }
    public decimal? NeckCircumference { get; set; }
    public decimal? ShoulderCircumference { get; set; }
    public string? Note { get; set; }
}
