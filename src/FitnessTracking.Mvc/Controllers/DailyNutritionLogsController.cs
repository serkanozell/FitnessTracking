using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class DailyNutritionLogsController(INutritionService nutritionService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await nutritionService.GetDailyLogsPagedAsync(page, pageSize, HttpContext.RequestAborted);
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new DailyNutritionLogEditModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DailyNutritionLogEditModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var id = await nutritionService.CreateDailyLogAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "Daily nutrition log created.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var log = await nutritionService.GetDailyLogByIdAsync(id, HttpContext.RequestAborted);
        if (log is null) return NotFound();

        var foods = await nutritionService.GetFoodsPagedAsync(1, 200, HttpContext.RequestAborted);
        ViewData["Foods"] = foods.Items;

        return View(log);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var log = await nutritionService.GetDailyLogByIdAsync(id, HttpContext.RequestAborted);
        if (log is null) return NotFound();

        var model = new DailyNutritionLogEditModel
        {
            Date = log.Date,
            DailyCalorieGoal = log.DailyCalorieGoal,
            Note = log.Note
        };
        ViewData["LogId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, DailyNutritionLogEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["LogId"] = id;
            return View(model);
        }

        var ok = await nutritionService.UpdateDailyLogAsync(id, model, HttpContext.RequestAborted);
        if (!ok) return NotFound();

        TempData["Success"] = "Daily nutrition log updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await nutritionService.DeleteDailyLogAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Daily nutrition log deleted.";
        return RedirectToAction(nameof(Index));
    }

    // —— LogEntry Operations ——

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEntry(Guid id, Guid foodId, decimal quantity)
    {
        await nutritionService.AddLogEntryAsync(id, foodId, quantity, HttpContext.RequestAborted);
        TempData["Success"] = "Food entry added.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEntry(Guid id, Guid entryId)
    {
        await nutritionService.RemoveLogEntryAsync(id, entryId, HttpContext.RequestAborted);
        TempData["Success"] = "Food entry removed.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateEntryQuantity(Guid id, Guid entryId, decimal quantity)
    {
        await nutritionService.UpdateLogEntryQuantityAsync(id, entryId, quantity, HttpContext.RequestAborted);
        TempData["Success"] = "Quantity updated.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
