using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class ExercisesController(IExercisesService exercisesService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await exercisesService.GetPagedAsync(page, pageSize, HttpContext.RequestAborted);
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new ExerciseEditModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExerciseEditModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await exercisesService.CreateAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "Exercise created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var exercise = await exercisesService.GetByIdAsync(id, HttpContext.RequestAborted);
        if (exercise is null) return NotFound();

        var model = new ExerciseEditModel
        {
            Name = exercise.Name,
            PrimaryMuscleGroup = exercise.PrimaryMuscleGroup,
            SecondaryMuscleGroup = exercise.SecondaryMuscleGroup,
            Description = exercise.Description
        };
        ViewData["ExerciseId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ExerciseEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ExerciseId"] = id;
            return View(model);
        }

        var ok = await exercisesService.UpdateAsync(id, model, HttpContext.RequestAborted);
        if (!ok) return NotFound();

        TempData["Success"] = "Exercise updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await exercisesService.ActivateAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Exercise activated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await exercisesService.DeleteAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Exercise deleted.";
        return RedirectToAction(nameof(Index));
    }
}
