using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface INutritionService
{
    // Foods
    Task<PagedResult<FoodDto>> GetFoodsPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<FoodDto?> GetFoodByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateFoodAsync(FoodEditModel model, CancellationToken ct = default);
    Task<bool> UpdateFoodAsync(Guid id, FoodEditModel model, CancellationToken ct = default);
    Task<bool> ActivateFoodAsync(Guid id, CancellationToken ct = default);
    Task<bool> DeleteFoodAsync(Guid id, CancellationToken ct = default);

    // MealPlans
    Task<PagedResult<MealPlanDto>> GetMealPlansPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<MealPlanDto?> GetMealPlanByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateMealPlanAsync(MealPlanEditModel model, CancellationToken ct = default);
    Task<bool> UpdateMealPlanAsync(Guid id, MealPlanEditModel model, CancellationToken ct = default);
    Task<bool> ActivateMealPlanAsync(Guid id, CancellationToken ct = default);
    Task<bool> DeleteMealPlanAsync(Guid id, CancellationToken ct = default);

    // Meals
    Task<Guid> AddMealAsync(Guid mealPlanId, string name, int order, CancellationToken ct = default);
    Task<bool> UpdateMealAsync(Guid mealPlanId, Guid mealId, string name, int order, CancellationToken ct = default);
    Task<bool> RemoveMealAsync(Guid mealPlanId, Guid mealId, CancellationToken ct = default);

    // MealItems
    Task<Guid> AddMealItemAsync(Guid mealPlanId, Guid mealId, Guid foodId, decimal quantity, CancellationToken ct = default);
    Task<bool> RemoveMealItemAsync(Guid mealPlanId, Guid mealId, Guid mealItemId, CancellationToken ct = default);
    Task<bool> UpdateMealItemQuantityAsync(Guid mealPlanId, Guid mealId, Guid mealItemId, decimal quantity, CancellationToken ct = default);

    // DailyNutritionLogs
    Task<PagedResult<DailyNutritionLogDto>> GetDailyLogsPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
    Task<DailyNutritionLogDto?> GetDailyLogByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateDailyLogAsync(DailyNutritionLogEditModel model, CancellationToken ct = default);
    Task<bool> UpdateDailyLogAsync(Guid id, DailyNutritionLogEditModel model, CancellationToken ct = default);
    Task<bool> DeleteDailyLogAsync(Guid id, CancellationToken ct = default);

    // LogEntries
    Task<Guid> AddLogEntryAsync(Guid logId, Guid foodId, decimal quantity, CancellationToken ct = default);
    Task<bool> RemoveLogEntryAsync(Guid logId, Guid entryId, CancellationToken ct = default);
    Task<bool> UpdateLogEntryQuantityAsync(Guid logId, Guid entryId, decimal quantity, CancellationToken ct = default);
}
