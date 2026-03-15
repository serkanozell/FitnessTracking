using BuildingBlocks.Domain.Abstractions;
using BodyMetrics.Domain.Events;

namespace BodyMetrics.Domain.Entity
{
    public class BodyMetric : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public DateTime Date { get; private set; }
        public decimal? Weight { get; private set; }
        public decimal? Height { get; private set; }
        public decimal? BodyFatPercentage { get; private set; }
        public decimal? MuscleMass { get; private set; }
        public decimal? WaistCircumference { get; private set; }
        public decimal? ChestCircumference { get; private set; }
        public decimal? ArmCircumference { get; private set; }
        public decimal? HipCircumference { get; private set; }
        public decimal? ThighCircumference { get; private set; }
        public decimal? NeckCircumference { get; private set; }
        public string? Note { get; private set; }

        private BodyMetric() { }

        public static BodyMetric Create(
            Guid userId,
            DateTime date,
            decimal? weight,
            decimal? height,
            decimal? bodyFatPercentage,
            decimal? muscleMass,
            decimal? waistCircumference,
            decimal? chestCircumference,
            decimal? armCircumference,
            decimal? hipCircumference,
            decimal? thighCircumference,
            decimal? neckCircumference,
            string? note)
        {
            var metric = new BodyMetric
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                Weight = weight,
                Height = height,
                BodyFatPercentage = bodyFatPercentage,
                MuscleMass = muscleMass,
                WaistCircumference = waistCircumference,
                ChestCircumference = chestCircumference,
                ArmCircumference = armCircumference,
                HipCircumference = hipCircumference,
                ThighCircumference = thighCircumference,
                NeckCircumference = neckCircumference,
                Note = note
            };

            metric.AddDomainEvent(new BodyMetricCreatedEvent(metric.Id, userId));

            return metric;
        }

        public void Update(
            DateTime date,
            decimal? weight,
            decimal? height,
            decimal? bodyFatPercentage,
            decimal? muscleMass,
            decimal? waistCircumference,
            decimal? chestCircumference,
            decimal? armCircumference,
            decimal? hipCircumference,
            decimal? thighCircumference,
            decimal? neckCircumference,
            string? note)
        {
            Date = date;
            Weight = weight;
            Height = height;
            BodyFatPercentage = bodyFatPercentage;
            MuscleMass = muscleMass;
            WaistCircumference = waistCircumference;
            ChestCircumference = chestCircumference;
            ArmCircumference = armCircumference;
            HipCircumference = hipCircumference;
            ThighCircumference = thighCircumference;
            NeckCircumference = neckCircumference;
            Note = note;

            AddDomainEvent(new BodyMetricUpdatedEvent(Id));
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new BodyMetricActivatedEvent(Id));
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;

            AddDomainEvent(new BodyMetricDeletedEvent(Id));
        }
    }
}
