using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class FoodsController(INutritionService nutritionService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await nutritionService.GetFoodsPagedAsync(page, pageSize, HttpContext.RequestAborted);
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new FoodEditModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FoodEditModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await nutritionService.CreateFoodAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "Food created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var food = await nutritionService.GetFoodByIdAsync(id, HttpContext.RequestAborted);
        if (food is null) return NotFound();

        var model = new FoodEditModel
        {
            Name = food.Name,
            Category = food.Category,
            DefaultServingSize = food.DefaultServingSize,
            ServingUnit = food.ServingUnit,
            Calories = food.Calories,
            Protein = food.Protein,
            Carbohydrates = food.Carbohydrates,
            Fat = food.Fat,
            Fiber = food.Fiber
        };
        ViewData["FoodId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, FoodEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["FoodId"] = id;
            return View(model);
        }

        var ok = await nutritionService.UpdateFoodAsync(id, model, HttpContext.RequestAborted);
        if (!ok) return NotFound();

        TempData["Success"] = "Food updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await nutritionService.ActivateFoodAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Food activated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await nutritionService.DeleteFoodAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Food deleted.";
        return RedirectToAction(nameof(Index));
    }
}
