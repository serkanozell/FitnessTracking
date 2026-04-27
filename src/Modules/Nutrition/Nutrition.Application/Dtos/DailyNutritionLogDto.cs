using Nutrition.Domain.Entity;

namespace Nutrition.Application.Dtos
{
    public sealed class DailyNutritionLogDto
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public DateTime Date { get; init; }
        public decimal? DailyCalorieGoal { get; init; }
        public string? Note { get; init; }
        public decimal TotalCalories { get; init; }
        public decimal TotalProtein { get; init; }
        public decimal TotalCarbohydrates { get; init; }
        public decimal TotalFat { get; init; }
        public decimal? RemainingCalories { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }
        public IReadOnlyList<LogEntryDto> Entries { get; init; } = [];

        public static DailyNutritionLogDto FromEntity(DailyNutritionLog entity)
        {
            var totalCalories = entity.Entries.Sum(e => e.Macros.Calories);

            return new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Date = entity.Date,
                DailyCalorieGoal = entity.DailyCalorieGoal,
                Note = entity.Note,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy,
                Entries = entity.Entries
                    .Select(LogEntryDto.FromEntity)
                    .ToList(),
                TotalCalories = totalCalories,
                TotalProtein = entity.Entries.Sum(e => e.Macros.Protein),
                TotalCarbohydrates = entity.Entries.Sum(e => e.Macros.Carbohydrates),
                TotalFat = entity.Entries.Sum(e => e.Macros.Fat),
                RemainingCalories = entity.DailyCalorieGoal.HasValue
                    ? entity.DailyCalorieGoal.Value - totalCalories
                    : null
            };
        }
    }

    public sealed class LogEntryDto
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

        public static LogEntryDto FromEntity(LogEntry entity) =>
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
