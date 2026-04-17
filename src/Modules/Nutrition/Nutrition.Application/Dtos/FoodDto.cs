using Nutrition.Domain.Entity;

namespace Nutrition.Application.Dtos
{
    public sealed class FoodDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Category { get; init; } = default!;
        public decimal DefaultServingSize { get; init; }
        public string ServingUnit { get; init; } = default!;
        public decimal Calories { get; init; }
        public decimal Protein { get; init; }
        public decimal Carbohydrates { get; init; }
        public decimal Fat { get; init; }
        public decimal? Fiber { get; init; }
        public Guid? UserId { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }

        public static FoodDto FromEntity(Food entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Category = entity.Category.ToString(),
                DefaultServingSize = entity.DefaultServingSize,
                ServingUnit = entity.ServingUnit.ToString(),
                Calories = entity.Macros.Calories,
                Protein = entity.Macros.Protein,
                Carbohydrates = entity.Macros.Carbohydrates,
                Fat = entity.Macros.Fat,
                Fiber = entity.Fiber,
                UserId = entity.UserId,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy
            };
    }
}
