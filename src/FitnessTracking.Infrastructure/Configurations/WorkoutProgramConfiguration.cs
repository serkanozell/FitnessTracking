using FitnessTracking.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WorkoutProgramConfiguration : IEntityTypeConfiguration<WorkoutProgram>
{
    public void Configure(EntityTypeBuilder<WorkoutProgram> builder)
    {
        builder.ToTable("WorkoutPrograms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.StartDate)
               .IsRequired();

        builder.Property(x => x.EndDate)
               .IsRequired();

        // Audit
        //builder.Property(x => x.CreatedDate)
        //    .IsRequired();

        //builder.Property(x => x.UpdatedDate);

        //builder.Property(x => x.CreatedBy)
        //    .HasMaxLength(100);

        //builder.Property(x => x.UpdatedBy)
        //    .HasMaxLength(100);

        builder.OwnsMany(
            x => x.WorkoutProgramExercises,
            pe =>
            {
                pe.ToTable("WorkoutProgramExercises");

                pe.WithOwner()
                    .HasForeignKey("WorkoutProgramId"); // shadow FK

                pe.HasKey(x => x.Id);

                pe.Property(x => x.Id)
                    .ValueGeneratedNever();

                pe.Property(x => x.ExerciseId)
                    .IsRequired();

                pe.Property(x => x.Sets)
                    .IsRequired();

                pe.Property(x => x.TargetReps)
                    .IsRequired();
            });
    }
}