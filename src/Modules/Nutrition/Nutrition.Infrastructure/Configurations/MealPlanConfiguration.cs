using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nutrition.Domain.Entity;

namespace Nutrition.Infrastructure.Configurations
{
    public sealed class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
    {
        private const string Schema = "nutrition";

        public void Configure(EntityTypeBuilder<MealPlan> builder)
        {
            builder.ToTable("MealPlans", schema: Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Date)
                   .IsRequired();

            builder.Property(x => x.Note)
                   .HasMaxLength(500);

            // Audit
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Meals (owned collection)
            builder.OwnsMany(
                x => x.Meals,
                meal =>
                {
                    meal.ToTable("Meals", schema: Schema);

                    meal.WithOwner()
                        .HasForeignKey("MealPlanId");

                    meal.HasKey(x => x.Id);

                    meal.Property(x => x.Id)
                        .ValueGeneratedNever();

                    meal.Property(x => x.MealPlanId)
                        .IsRequired();

                    meal.Property(x => x.Name)
                        .IsRequired()
                        .HasMaxLength(100);

                    meal.Property(x => x.Order)
                        .IsRequired();

                    // Audit
                    meal.Property(x => x.CreatedDate).IsRequired();
                    meal.Property(x => x.UpdatedDate);
                    meal.Property(x => x.CreatedBy).HasMaxLength(100);
                    meal.Property(x => x.UpdatedBy).HasMaxLength(100);

                    // MealItems under each Meal
                    meal.OwnsMany(
                        m => m.MealItems,
                        item =>
                        {
                            item.ToTable("MealItems", schema: Schema);

                            item.WithOwner()
                                .HasForeignKey("MealId");

                            item.HasKey(x => x.Id);

                            item.Property(x => x.Id)
                                .ValueGeneratedNever();

                            item.Property(x => x.MealId)
                                .IsRequired();

                            item.Property(x => x.FoodId)
                                .IsRequired();

                            item.Property(x => x.FoodName)
                                .IsRequired()
                                .HasMaxLength(200);

                            item.Property(x => x.Quantity)
                                .IsRequired()
                                .HasPrecision(10, 2);

                            item.Property(x => x.ServingUnit)
                                .IsRequired()
                                .HasConversion<string>()
                                .HasMaxLength(50);

                            item.OwnsOne(x => x.Macros, m =>
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
                            item.Property(x => x.CreatedDate).IsRequired();
                            item.Property(x => x.UpdatedDate);
                            item.Property(x => x.CreatedBy).HasMaxLength(100);
                            item.Property(x => x.UpdatedBy).HasMaxLength(100);
                        });
                });
        }
    }
}
