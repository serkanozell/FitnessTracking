using BodyMetrics.Domain.Entity;

namespace BodyMetrics.Application.Dtos
{
    public sealed class BodyMetricDto
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
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

        public static BodyMetricDto FromEntity(BodyMetric entity) =>
            new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Date = entity.Date,
                Weight = entity.Weight?.Value,
                Height = entity.Height?.Value,
                BodyFatPercentage = entity.BodyFatPercentage?.Value,
                MuscleMass = entity.MuscleMass,
                WaistCircumference = entity.WaistCircumference,
                ChestCircumference = entity.ChestCircumference,
                ArmCircumference = entity.ArmCircumference,
                HipCircumference = entity.HipCircumference,
                ThighCircumference = entity.ThighCircumference,
                NeckCircumference = entity.NeckCircumference,
                ShoulderCircumference = entity.ShoulderCircumference,
                Note = entity.Note,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate
            };
    }
}
