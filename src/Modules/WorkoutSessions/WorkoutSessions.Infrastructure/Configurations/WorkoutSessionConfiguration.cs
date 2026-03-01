using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Infrastructure.Configurations
{
    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        private const string Schema = "workoutsessions";

        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable(name: "WorkoutSessions", schema: Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkoutProgramId)
                   .IsRequired();

            builder.Property(x => x.Date)
                   .IsRequired();

            builder.HasIndex(x => new { x.WorkoutProgramId, x.Date })
                   .IsUnique();

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

            // Owned Entity
            builder.OwnsMany(
                x => x.SessionExercises,
                we =>
                {
                    we.ToTable(name: "WorkoutSessionExercises", schema: Schema);

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

                    we.Property(x => x.CreatedDate)
                      .IsRequired();

                    we.Property(x => x.UpdatedDate);

                    we.Property(x => x.CreatedBy)
                        .HasMaxLength(100);

                    we.Property(x => x.UpdatedBy)
                        .HasMaxLength(100);
                });
        }
    }
}