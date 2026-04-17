namespace Nutrition.Contracts;

public record DailyNutritionSummary(
    DateTime Date,
    decimal TotalCalories,
    decimal TotalProtein,
    decimal TotalCarbohydrates,
    decimal TotalFat,
    int MealCount);
