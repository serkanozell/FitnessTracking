using Nutrition.Domain.Entity;

namespace Nutrition.Application.Dtos
{
    public sealed class MealPlanDto
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Name { get; init; } = default!;
        public DateTime Date { get; init; }
        public string? Note { get; init; }
        public decimal TotalCalories { get; init; }
        public decimal TotalProtein { get; init; }
        public decimal TotalCarbohydrates { get; init; }
        public decimal TotalFat { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }
        public IReadOnlyList<MealDto> Meals { get; init; } = [];

        public static MealPlanDto FromEntity(MealPlan entity) =>
            new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Date = entity.Date,
                Note = entity.Note,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy,
                Meals = entity.Meals
                    .Select(MealDto.FromEntity)
                    .OrderBy(m => m.Order)
                    .ToList(),
                TotalCalories = entity.Meals.SelectMany(m => m.MealItems).Sum(i => i.Macros.Calories),
                TotalProtein = entity.Meals.SelectMany(m => m.MealItems).Sum(i => i.Macros.Protein),
                TotalCarbohydrates = entity.Meals.SelectMany(m => m.MealItems).Sum(i => i.Macros.Carbohydrates),
                TotalFat = entity.Meals.SelectMany(m => m.MealItems).Sum(i => i.Macros.Fat)
            };
    }

    public sealed class MealDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public int Order { get; init; }
        public decimal MealCalories { get; init; }
        public decimal MealProtein { get; init; }
        public decimal MealCarbohydrates { get; init; }
        public decimal MealFat { get; init; }
        public IReadOnlyList<MealItemDto> Items { get; init; } = [];

        public static MealDto FromEntity(Meal entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Order = entity.Order,
                Items = entity.MealItems
                    .Select(MealItemDto.FromEntity)
                    .ToList(),
                MealCalories = entity.MealItems.Sum(i => i.Macros.Calories),
                MealProtein = entity.MealItems.Sum(i => i.Macros.Protein),
                MealCarbohydrates = entity.MealItems.Sum(i => i.Macros.Carbohydrates),
                MealFat = entity.MealItems.Sum(i => i.Macros.Fat)
            };
    }

    public sealed class MealItemDto
    {
        public Guid Id { get; init; }
        public Guid FoodId { get; init; }
        public string FoodName { get; init; } = default!;
        public decimal Quantity { get; init; }
        public string ServingUnit { get; init; } = default!;
        public decimal Calories { get; init; }
        public decimal Protein { get; init; }
        public decimal Carbohydrates { get; init; }
        public decimal Fat { get; init; }

        public static MealItemDto FromEntity(MealItem entity) =>
            new()
            {
                Id = entity.Id,
                FoodId = entity.FoodId,
                FoodName = entity.FoodName,
                Quantity = entity.Quantity,
                ServingUnit = entity.ServingUnit.ToString(),
                Calories = entity.Macros.Calories,
                Protein = entity.Macros.Protein,
                Carbohydrates = entity.Macros.Carbohydrates,
                Fat = entity.Macros.Fat
            };
    }
}
