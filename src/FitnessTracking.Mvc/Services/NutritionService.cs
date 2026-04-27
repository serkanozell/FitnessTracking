using System.Net;
using System.Net.Http.Json;
using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public sealed class NutritionService(HttpClient httpClient) : INutritionService
{
    private const string FoodsUrl = "api/v1/foods";
    private const string MealPlansUrl = "api/v1/meal-plans";
    private const string DailyLogsUrl = "api/v1/daily-nutrition-logs";

    // ── Foods ──

    public async Task<PagedResult<FoodDto>> GetFoodsPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await httpClient.GetFromJsonAsync<PagedResult<FoodDto>>(
            $"{FoodsUrl}?pageNumber={pageNumber}&pageSize={pageSize}", ct);
        return result ?? new PagedResult<FoodDto>();
    }

    public async Task<FoodDto?> GetFoodByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync($"{FoodsUrl}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<FoodDto>(cancellationToken: ct);
    }

    public async Task<Guid> CreateFoodAsync(FoodEditModel model, CancellationToken ct = default)
    {
        var payload = new
        {
            model.Name, model.Category, model.DefaultServingSize, model.ServingUnit,
            model.Calories, model.Protein, model.Carbohydrates, model.Fat, model.Fiber
        };
        using var response = await httpClient.PostAsJsonAsync(FoodsUrl, payload, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateFoodResponse>(cancellationToken: ct);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateFoodAsync(Guid id, FoodEditModel model, CancellationToken ct = default)
    {
        var payload = new
        {
            model.Name, model.Category, model.DefaultServingSize, model.ServingUnit,
            model.Calories, model.Protein, model.Carbohydrates, model.Fat, model.Fiber
        };
        using var response = await httpClient.PutAsJsonAsync($"{FoodsUrl}/{id}", payload, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ActivateFoodAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.PutAsync($"{FoodsUrl}/{id}/activate", null, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteFoodAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync($"{FoodsUrl}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    // ── MealPlans ──

    public async Task<PagedResult<MealPlanDto>> GetMealPlansPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await httpClient.GetFromJsonAsync<PagedResult<MealPlanDto>>(
            $"{MealPlansUrl}?pageNumber={pageNumber}&pageSize={pageSize}", ct);
        return result ?? new PagedResult<MealPlanDto>();
    }

    public async Task<MealPlanDto?> GetMealPlanByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync($"{MealPlansUrl}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MealPlanDto>(cancellationToken: ct);
    }

    public async Task<Guid> CreateMealPlanAsync(MealPlanEditModel model, CancellationToken ct = default)
    {
        var payload = new { model.Name, model.Date, model.Note };
        using var response = await httpClient.PostAsJsonAsync(MealPlansUrl, payload, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateMealPlanResponse>(cancellationToken: ct);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateMealPlanAsync(Guid id, MealPlanEditModel model, CancellationToken ct = default)
    {
        var payload = new { model.Name, model.Date, model.Note };
        using var response = await httpClient.PutAsJsonAsync($"{MealPlansUrl}/{id}", payload, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> ActivateMealPlanAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.PutAsync($"{MealPlansUrl}/{id}/activate", null, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteMealPlanAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync($"{MealPlansUrl}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    // ── Meals ──

    public async Task<Guid> AddMealAsync(Guid mealPlanId, string name, int order, CancellationToken ct = default)
    {
        var payload = new { Name = name, Order = order };
        using var response = await httpClient.PostAsJsonAsync($"{MealPlansUrl}/{mealPlanId}/meals", payload, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AddMealResponse>(cancellationToken: ct);
        return result?.MealId ?? Guid.Empty;
    }

    public async Task<bool> UpdateMealAsync(Guid mealPlanId, Guid mealId, string name, int order, CancellationToken ct = default)
    {
        var payload = new { Name = name, Order = order };
        using var response = await httpClient.PutAsJsonAsync($"{MealPlansUrl}/{mealPlanId}/meals/{mealId}", payload, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> RemoveMealAsync(Guid mealPlanId, Guid mealId, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync($"{MealPlansUrl}/{mealPlanId}/meals/{mealId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    // ── MealItems ──

    public async Task<Guid> AddMealItemAsync(Guid mealPlanId, Guid mealId, Guid foodId, decimal quantity, CancellationToken ct = default)
    {
        var payload = new { FoodId = foodId, Quantity = quantity };
        using var response = await httpClient.PostAsJsonAsync($"{MealPlansUrl}/{mealPlanId}/meals/{mealId}/items", payload, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AddMealItemResponse>(cancellationToken: ct);
        return result?.MealItemId ?? Guid.Empty;
    }

    public async Task<bool> RemoveMealItemAsync(Guid mealPlanId, Guid mealId, Guid mealItemId, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync($"{MealPlansUrl}/{mealPlanId}/meals/{mealId}/items/{mealItemId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> UpdateMealItemQuantityAsync(Guid mealPlanId, Guid mealId, Guid mealItemId, decimal quantity, CancellationToken ct = default)
    {
        var payload = new { Quantity = quantity };
        using var response = await httpClient.PutAsJsonAsync($"{MealPlansUrl}/{mealPlanId}/meals/{mealId}/items/{mealItemId}", payload, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    // —— DailyNutritionLogs ——

    public async Task<PagedResult<DailyNutritionLogDto>> GetDailyLogsPagedAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await httpClient.GetFromJsonAsync<PagedResult<DailyNutritionLogDto>>(
            $"{DailyLogsUrl}?pageNumber={pageNumber}&pageSize={pageSize}", ct);
        return result ?? new PagedResult<DailyNutritionLogDto>();
    }

    public async Task<DailyNutritionLogDto?> GetDailyLogByIdAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.GetAsync($"{DailyLogsUrl}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DailyNutritionLogDto>(cancellationToken: ct);
    }

    public async Task<Guid> CreateDailyLogAsync(DailyNutritionLogEditModel model, CancellationToken ct = default)
    {
        var payload = new { model.Date, model.DailyCalorieGoal, model.Note };
        using var response = await httpClient.PostAsJsonAsync(DailyLogsUrl, payload, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CreateDailyLogResponse>(cancellationToken: ct);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<bool> UpdateDailyLogAsync(Guid id, DailyNutritionLogEditModel model, CancellationToken ct = default)
    {
        var payload = new { model.DailyCalorieGoal, model.Note };
        using var response = await httpClient.PutAsJsonAsync($"{DailyLogsUrl}/{id}", payload, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> DeleteDailyLogAsync(Guid id, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync($"{DailyLogsUrl}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    // —— LogEntries ——

    public async Task<Guid> AddLogEntryAsync(Guid logId, Guid foodId, decimal quantity, CancellationToken ct = default)
    {
        var payload = new { FoodId = foodId, Quantity = quantity };
        using var response = await httpClient.PostAsJsonAsync($"{DailyLogsUrl}/{logId}/entries", payload, ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AddLogEntryResponse>(cancellationToken: ct);
        return result?.EntryId ?? Guid.Empty;
    }

    public async Task<bool> RemoveLogEntryAsync(Guid logId, Guid entryId, CancellationToken ct = default)
    {
        using var response = await httpClient.DeleteAsync($"{DailyLogsUrl}/{logId}/entries/{entryId}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> UpdateLogEntryQuantityAsync(Guid logId, Guid entryId, decimal quantity, CancellationToken ct = default)
    {
        var payload = new { Quantity = quantity };
        using var response = await httpClient.PutAsJsonAsync($"{DailyLogsUrl}/{logId}/entries/{entryId}/quantity", payload, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }
}
