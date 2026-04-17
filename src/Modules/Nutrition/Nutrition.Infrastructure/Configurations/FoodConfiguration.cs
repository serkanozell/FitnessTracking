using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrition.Domain.Entity;

namespace Nutrition.Infrastructure.Configurations
{
    public sealed class FoodConfiguration : IEntityTypeConfiguration<Food>
    {
        private const string Schema = "nutrition";

        public void Configure(EntityTypeBuilder<Food> builder)
        {
            builder.ToTable("Foods", schema: Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Category)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.DefaultServingSize)
                   .IsRequired()
                   .HasPrecision(10, 2);

            builder.Property(x => x.ServingUnit)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.OwnsOne(x => x.Macros, m =>
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

            builder.Property(x => x.Fiber)
                   .HasPrecision(10, 2);

            builder.Property(x => x.UserId);

            builder.HasIndex(x => x.UserId);

            // Audit
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
