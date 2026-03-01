using Exercises.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exercises.Infrastructure.Configurations
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.ToTable(name: "Exercises", schema: "exercises");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(x => x.PrimaryMuscleGroup)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.SecondaryMuscleGroup)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            // Audit
            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.UpdatedDate);

            builder.Property(x => x.CreatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}