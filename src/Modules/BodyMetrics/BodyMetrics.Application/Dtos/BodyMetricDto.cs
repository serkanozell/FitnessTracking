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
        public string? Note { get; init; }
        public DateTime? CreatedDate { get; init; }

        public static BodyMetricDto FromEntity(BodyMetric entity) =>
            new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Date = entity.Date,
                Weight = entity.Weight,
                Height = entity.Height,
                BodyFatPercentage = entity.BodyFatPercentage,
                MuscleMass = entity.MuscleMass,
                WaistCircumference = entity.WaistCircumference,
                ChestCircumference = entity.ChestCircumference,
                ArmCircumference = entity.ArmCircumference,
                HipCircumference = entity.HipCircumference,
                ThighCircumference = entity.ThighCircumference,
                NeckCircumference = entity.NeckCircumference,
                Note = entity.Note,
                CreatedDate = entity.CreatedDate
            };
    }
}
