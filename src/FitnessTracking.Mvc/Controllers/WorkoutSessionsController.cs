using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class WorkoutSessionsController(
    IWorkoutSessionsService sessionsService,
    IWorkoutProgramsService programsService,
    IExercisesService exercisesService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await sessionsService.GetPagedAsync(page, pageSize, HttpContext.RequestAborted);
        var programs = await programsService.GetPagedAsync(1, 100, HttpContext.RequestAborted);
        var allExercises = await exercisesService.GetPagedAsync(1, 100, HttpContext.RequestAborted);

        ViewData["Programs"] = programs.Items;
        ViewData["AllExercises"] = allExercises.Items;

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var programs = await programsService.GetPagedAsync(1, 100, HttpContext.RequestAborted);
        ViewData["Programs"] = programs.Items;
        return View(new WorkoutSessionEditModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutSessionEditModel model)
    {
        if (!ModelState.IsValid)
        {
            var programs = await programsService.GetPagedAsync(1, 100, HttpContext.RequestAborted);
            ViewData["Programs"] = programs.Items;
            return View(model);
        }

        var id = await sessionsService.CreateAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "Session created.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var session = await sessionsService.GetByIdAsync(id, HttpContext.RequestAborted);
        if (session is null) return NotFound();

        var allExercises = await exercisesService.GetPagedAsync(1, 100, HttpContext.RequestAborted);
        var programs = await programsService.GetPagedAsync(1, 100, HttpContext.RequestAborted);

        ViewData["AllExercises"] = allExercises.Items;
        ViewData["Programs"] = programs.Items;
        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExercise(Guid id, Guid exerciseId, int setNumber, decimal weight, int reps, string? returnTo, int? page, int? pageSize)
    {
        var model = new WorkoutExerciseEditModel
        {
            ExerciseId = exerciseId,
            SetNumber = setNumber,
            Weight = weight,
            Reps = reps
        };
        await sessionsService.AddWorkoutExerciseAsync(id, model, HttpContext.RequestAborted);
        TempData["Success"] = "Set added.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveExercise(Guid id, Guid exerciseId, string? returnTo, int? page, int? pageSize)
    {
        await sessionsService.DeleteWorkoutExerciseAsync(id, exerciseId, HttpContext.RequestAborted);
        TempData["Success"] = "Exercise removed.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await sessionsService.ActivateAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Session activated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateExercise(Guid id, Guid exerciseId, string? returnTo, int? page, int? pageSize)
    {
        await sessionsService.ActivateWorkoutExerciseAsync(id, exerciseId, HttpContext.RequestAborted);
        TempData["Success"] = "Exercise activated.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await sessionsService.DeleteAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Session deleted.";
        return RedirectToAction(nameof(Index));
    }

    private IActionResult RedirectBack(Guid sessionId, string? returnTo, int? page, int? pageSize)
    {
        if (returnTo == "Index")
            return RedirectToAction(nameof(Index), new { page, pageSize, expand = sessionId });
        return RedirectToAction(nameof(Details), new { id = sessionId });
    }
}
