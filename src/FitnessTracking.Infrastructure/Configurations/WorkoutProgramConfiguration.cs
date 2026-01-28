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

        // WorkoutProgramSplits (owned collection)
        builder.OwnsMany(
            x => x.Splits,
            split =>
            {
                split.ToTable("WorkoutProgramSplits");

                split.WithOwner()
                     .HasForeignKey("WorkoutProgramId"); // shadow FK to WorkoutProgram

                split.HasKey(x => x.Id);

                split.Property(x => x.Id)
                     .ValueGeneratedNever();

                split.Property(x => x.WorkoutProgramId)
                     .IsRequired();

                split.Property(x => x.Name)
                     .IsRequired()
                     .HasMaxLength(100);

                split.Property(x => x.Order)
                     .IsRequired();

                // WorkoutProgramExercises under each split
                split.OwnsMany(
                    s => s.Exercises,
                    pe =>
                    {
                        pe.ToTable("WorkoutProgramSplitExercises");

                        pe.WithOwner()
                          .HasForeignKey("WorkoutProgramSplitId"); // FK to split

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
            });
    }
}