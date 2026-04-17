using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class MealPlansController(INutritionService nutritionService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await nutritionService.GetMealPlansPagedAsync(page, pageSize, HttpContext.RequestAborted);
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new MealPlanEditModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MealPlanEditModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var id = await nutritionService.CreateMealPlanAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "Meal plan created.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var plan = await nutritionService.GetMealPlanByIdAsync(id, HttpContext.RequestAborted);
        if (plan is null) return NotFound();

        var foods = await nutritionService.GetFoodsPagedAsync(1, 200, HttpContext.RequestAborted);
        ViewData["Foods"] = foods.Items;

        return View(plan);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var plan = await nutritionService.GetMealPlanByIdAsync(id, HttpContext.RequestAborted);
        if (plan is null) return NotFound();

        var model = new MealPlanEditModel
        {
            Name = plan.Name,
            Date = plan.Date,
            Note = plan.Note
        };
        ViewData["PlanId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, MealPlanEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PlanId"] = id;
            return View(model);
        }

        var ok = await nutritionService.UpdateMealPlanAsync(id, model, HttpContext.RequestAborted);
        if (!ok) return NotFound();

        TempData["Success"] = "Meal plan updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await nutritionService.ActivateMealPlanAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Meal plan activated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await nutritionService.DeleteMealPlanAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Meal plan deleted.";
        return RedirectToAction(nameof(Index));
    }

    // ── Meal Operations ──

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMeal(Guid id, string name, int order)
    {
        await nutritionService.AddMealAsync(id, name, order, HttpContext.RequestAborted);
        TempData["Success"] = "Meal added.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMeal(Guid id, Guid mealId)
    {
        await nutritionService.RemoveMealAsync(id, mealId, HttpContext.RequestAborted);
        TempData["Success"] = "Meal removed.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ── MealItem Operations ──

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMealItem(Guid id, Guid mealId, Guid foodId, decimal quantity)
    {
        await nutritionService.AddMealItemAsync(id, mealId, foodId, quantity, HttpContext.RequestAborted);
        TempData["Success"] = "Food item added.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMealItem(Guid id, Guid mealId, Guid mealItemId)
    {
        await nutritionService.RemoveMealItemAsync(id, mealId, mealItemId, HttpContext.RequestAborted);
        TempData["Success"] = "Food item removed.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateMealItemQuantity(Guid id, Guid mealId, Guid mealItemId, decimal quantity)
    {
        await nutritionService.UpdateMealItemQuantityAsync(id, mealId, mealItemId, quantity, HttpContext.RequestAborted);
        TempData["Success"] = "Quantity updated.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
