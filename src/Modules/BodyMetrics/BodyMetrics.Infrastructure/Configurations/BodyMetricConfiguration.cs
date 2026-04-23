using BodyMetrics.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BodyMetrics.Infrastructure.Configurations
{
    public class BodyMetricConfiguration : IEntityTypeConfiguration<BodyMetric>
    {
        private const string Schema = "bodymetrics";

        public void Configure(EntityTypeBuilder<BodyMetric> builder)
        {
            builder.ToTable(name: "BodyMetrics", schema: Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Date).IsRequired();

            builder.OwnsOne(x => x.Weight, w =>
            {
                w.Property(x => x.Value)
                 .HasColumnName("Weight")
                 .HasPrecision(5, 2);
            });

            builder.OwnsOne(x => x.Height, h =>
            {
                h.Property(x => x.Value)
                 .HasColumnName("Height")
                 .HasPrecision(5, 2);
            });

            builder.OwnsOne(x => x.BodyFatPercentage, p =>
            {
                p.Property(x => x.Value)
                 .HasColumnName("BodyFatPercentage")
                 .HasPrecision(5, 2);
            });

            builder.Property(x => x.MuscleMass).HasPrecision(5, 2);
            builder.Property(x => x.WaistCircumference).HasPrecision(5, 2);
            builder.Property(x => x.ChestCircumference).HasPrecision(5, 2);
            builder.Property(x => x.ArmCircumference).HasPrecision(5, 2);
            builder.Property(x => x.HipCircumference).HasPrecision(5, 2);
            builder.Property(x => x.ThighCircumference).HasPrecision(5, 2);
            builder.Property(x => x.NeckCircumference).HasPrecision(5, 2);
            builder.Property(x => x.ShoulderCircumference).HasPrecision(5, 2);

            builder.Property(x => x.Note).HasMaxLength(500);

            // Audit
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}