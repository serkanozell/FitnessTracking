using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrition.Domain.Entity;

namespace Nutrition.Infrastructure.Configurations
{
    public sealed class DailyNutritionLogConfiguration : IEntityTypeConfiguration<DailyNutritionLog>
    {
        private const string Schema = "nutrition";

        public void Configure(EntityTypeBuilder<DailyNutritionLog> builder)
        {
            builder.ToTable("DailyNutritionLogs", schema: Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.Date })
                   .IsUnique();

            builder.Property(x => x.Date)
                   .IsRequired();

            builder.Property(x => x.DailyCalorieGoal)
                   .HasPrecision(10, 2);

            builder.Property(x => x.Note)
                   .HasMaxLength(500);

            // Audit
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // LogEntries (owned collection)
            builder.OwnsMany(
                x => x.Entries,
                entry =>
                {
                    entry.ToTable("LogEntries", schema: Schema);

                    entry.WithOwner()
                         .HasForeignKey("DailyNutritionLogId");

                    entry.HasKey(x => x.Id);

                    entry.Property(x => x.Id)
                         .ValueGeneratedNever();

                    entry.Property(x => x.DailyNutritionLogId)
                         .IsRequired();

                    entry.Property(x => x.FoodId)
                         .IsRequired();

                    entry.Property(x => x.FoodName)
                         .IsRequired()
                         .HasMaxLength(200);

                    entry.Property(x => x.Quantity)
                         .IsRequired()
                         .HasPrecision(10, 2);

                    entry.Property(x => x.ServingUnit)
                         .IsRequired()
                         .HasConversion<string>()
                         .HasMaxLength(50);

                    entry.OwnsOne(x => x.Macros, m =>
                    {
                        m.Property(x => x.Calories)
                         .HasColumnName("Calories")
                         .HasPrecision(10, 2)
                         .IsRequired();

                        m.Property(x => x.Protein)
                         .HasColumnName("Protein")
                         .HasPrecision(10, 2)
                         .IsRequired();

                        m.Property(x => x.Carbohydrates)
                         .HasColumnName("Carbohydrates")
                         .HasPrecision(10, 2)
                         .IsRequired();

                        m.Property(x => x.Fat)
                         .HasColumnName("Fat")
                         .HasPrecision(10, 2)
                         .IsRequired();
                    });

                    // Audit
                    entry.Property(x => x.CreatedDate).IsRequired();
                    entry.Property(x => x.UpdatedDate);
                    entry.Property(x => x.CreatedBy).HasMaxLength(100);
                    entry.Property(x => x.UpdatedBy).HasMaxLength(100);
                });
        }
    }
}
