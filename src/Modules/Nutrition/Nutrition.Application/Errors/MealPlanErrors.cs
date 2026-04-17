namespace Nutrition.Application.Errors
{
    public static class MealPlanErrors
    {
        public static Error NotFound(Guid id) =>
            new("MealPlan.NotFound", $"Meal plan with ID '{id}' was not found.");

        public static Error AlreadyActive(Guid id) =>
            new("MealPlan.AlreadyActive", $"Meal plan with ID '{id}' is already active.");

        public static Error AlreadyDeleted(Guid id) =>
            new("MealPlan.AlreadyDeleted", $"Meal plan with ID '{id}' is already deleted.");

        public static Error MealNotFound(Guid mealPlanId, Guid mealId) =>
            new("MealPlan.MealNotFound", $"Meal with ID '{mealId}' was not found in meal plan '{mealPlanId}'.");

        public static Error MealItemNotFound(Guid mealId, Guid mealItemId) =>
            new("MealPlan.MealItemNotFound", $"Meal item with ID '{mealItemId}' was not found in meal '{mealId}'.");
    }
}
