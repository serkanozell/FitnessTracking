using FitnessTracking.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.ToTable("WorkoutSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkoutProgramId)
               .IsRequired();

        builder.Property(x => x.Date)
               .IsRequired();

        builder.HasIndex(x => new { x.WorkoutProgramId, x.Date })
               .IsUnique();

        //builder.HasMany(x => x.WorkoutExercises)
        //       .WithOne()
        //       .HasForeignKey("WorkoutSessionId")
        //       .OnDelete(DeleteBehavior.Cascade);

        // Audit
        //builder.Property(x => x.CreatedDate)
        //    .IsRequired();

        //builder.Property(x => x.UpdatedDate);

        //builder.Property(x => x.CreatedBy)
        //    .HasMaxLength(100);

        //builder.Property(x => x.UpdatedBy)
        //    .HasMaxLength(100);

        // Owned Entity
        builder.OwnsMany(
            x => x.WorkoutExercises,
            we =>
            {
                we.ToTable("WorkoutExercises");

                we.WithOwner()
                  .HasForeignKey("WorkoutSessionId"); // shadow FK

                we.HasKey(x => x.Id);

                we.Property(x => x.Id)
                  .ValueGeneratedNever();

                we.Property(x => x.ExerciseId)
                  .IsRequired();

                we.Property(x => x.SetNumber)
                  .IsRequired();

                we.Property(x => x.Weight)
                  .IsRequired()
                  .HasColumnType("decimal(8,2)");

                we.Property(x => x.Reps)
                  .IsRequired();
            });
    }
}
