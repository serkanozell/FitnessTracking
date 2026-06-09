using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Infrastructure.Persistence;

namespace WorkoutSessions.Infrastructure.Configurations
{
    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        private const string Schema = WorkoutSessionsSchema.Name;

        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable(name: "WorkoutSessions", schema: Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.HasIndex(x => x.UserId);

            // All analytics queries (volume trend, exercise progress, personal records)
            // filter sessions by UserId + Date range AND !IsDeleted. A composite filtered
            // index keeps these range scans selective over long periods (e.g. 365 days)
            // while excluding soft-deleted rows from the index altogether.
            builder.HasIndex(x => new { x.UserId, x.Date })
                   .HasFilter("[IsDeleted] = 0");

            builder.Property(x => x.WorkoutProgramId)
                   .IsRequired();

            builder.Property(x => x.WorkoutProgramSplitId)
                   .IsRequired();

            builder.HasIndex(x => x.WorkoutProgramSplitId)
                   .HasFilter("[IsDeleted] = 0");

            // GetActiveByProgramIdAsync / GetListByProgramAsync filter by WorkoutProgramId
            // (+ !IsDeleted) without a Date predicate, so the unique (WorkoutProgramId, Date)
            // index's leftmost prefix is not a filtered match. A dedicated filtered index on
            // WorkoutProgramId serves those soft-delete-aware lookups.
            builder.HasIndex(x => x.WorkoutProgramId)
                   .HasFilter("[IsDeleted] = 0");

            builder.Property(x => x.Date)
                   .IsRequired();

            builder.HasIndex(x => new { x.WorkoutProgramId, x.Date })
                   .IsUnique();

            // Audit
            builder.Property(x => x.CreatedDate);

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