using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class WorkoutProgramsController(
    IWorkoutProgramsService programsService,
    IExercisesService exercisesService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var ct = HttpContext.RequestAborted;

        // Independent lookups; run them in parallel to halve wait time.
        var resultTask = programsService.GetPagedAsync(page, pageSize, ct);
        var allExercisesTask = exercisesService.GetPagedAsync(1, 100, ct);

        await Task.WhenAll(resultTask, allExercisesTask);

        ViewData["AllExercises"] = allExercisesTask.Result.Items;

        return View(resultTask.Result);
    }

    [HttpGet]
    public IActionResult Create() => View(new WorkoutProgramEditModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutProgramEditModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var id = await programsService.CreateAsync(
            new CreateWorkoutProgramRequest { Name = model.Name, Description = model.Description, StartDate = model.StartDate, EndDate = model.EndDate },
            HttpContext.RequestAborted);

        TempData["Success"] = "Program created.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var program = await programsService.GetByIdAsync(id, HttpContext.RequestAborted);
        if (program is null) return NotFound();

        var model = new WorkoutProgramEditModel
        {
            Name = program.Name,
            Description = program.Description,
            StartDate = program.StartDate,
            EndDate = program.EndDate
        };
        ViewData["ProgramId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, WorkoutProgramEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ProgramId"] = id;
            return View(model);
        }

        await programsService.UpdateAsync(id,
            new UpdateWorkoutProgramRequest { Name = model.Name, Description = model.Description, StartDate = model.StartDate, EndDate = model.EndDate },
            HttpContext.RequestAborted);

        TempData["Success"] = "Program updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await programsService.ActivateAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Program activated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await programsService.DeleteAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Program deleted.";
        return RedirectToAction(nameof(Index));
    }

    // --- Details with Splits ---

    public async Task<IActionResult> Details(Guid id)
    {
        var view = await programsService.GetDetailViewAsync(id, HttpContext.RequestAborted);
        if (view is null) return NotFound();
        return View(view);
    }

    // --- Split CRUD ---

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSplit(Guid id, string name, int order, string? returnTo, int? page, int? pageSize)
    {
        await programsService.AddSplitAsync(id, new AddSplitRequest { Name = name, Order = order }, HttpContext.RequestAborted);
        TempData["Success"] = "Split added.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSplit(Guid id, Guid splitId, string name, int order, string? returnTo, int? page, int? pageSize)
    {
        await programsService.UpdateSplitAsync(id, splitId, new UpdateSplitRequest { Name = name, Order = order }, HttpContext.RequestAborted);
        TempData["Success"] = "Split updated.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSplit(Guid id, Guid splitId, string? returnTo, int? page, int? pageSize)
    {
        await programsService.DeleteSplitAsync(id, splitId, HttpContext.RequestAborted);
        TempData["Success"] = "Split deleted.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateSplit(Guid id, Guid splitId, string? returnTo, int? page, int? pageSize)
    {
        await programsService.ActivateSplitAsync(id, splitId, HttpContext.RequestAborted);
        TempData["Success"] = "Split activated.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    // --- Split Exercise CRUD ---

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExerciseToSplit(Guid id, Guid splitId, Guid exerciseId, int sets, int minimumReps, int maximumReps, string? returnTo, int? page, int? pageSize)
    {
        await programsService.AddExerciseToSplitAsync(id, splitId,
            new AddProgramExerciseRequest { ExerciseId = exerciseId, Sets = sets, MinimumReps = minimumReps, MaximumReps = maximumReps },
            HttpContext.RequestAborted);
        TempData["Success"] = "Exercise added to split.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSplitExercise(Guid id, Guid splitId, Guid exerciseId, int sets, int minimumReps, int maximumReps, string? returnTo, int? page, int? pageSize)
    {
        await programsService.UpdateExerciseInSplitAsync(id, splitId, exerciseId,
            new UpdateProgramExerciseRequest { Sets = sets, MinimumReps = minimumReps, MaximumReps = maximumReps },
            HttpContext.RequestAborted);
        TempData["Success"] = "Exercise updated.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveExerciseFromSplit(Guid id, Guid splitId, Guid exerciseId, string? returnTo, int? page, int? pageSize)
    {
        await programsService.RemoveExerciseFromSplitAsync(id, splitId, exerciseId, HttpContext.RequestAborted);
        TempData["Success"] = "Exercise removed.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateSplitExercise(Guid id, Guid splitId, Guid exerciseId, string? returnTo, int? page, int? pageSize)
    {
        await programsService.ActivateSplitExerciseAsync(id, splitId, exerciseId, HttpContext.RequestAborted);
        TempData["Success"] = "Exercise activated.";
        return RedirectBack(id, returnTo, page, pageSize);
    }

    private IActionResult RedirectBack(Guid programId, string? returnTo, int? page, int? pageSize)
    {
        if (returnTo == "Index")
            return RedirectToAction(nameof(Index), new { page, pageSize, expand = programId });
        return RedirectToAction(nameof(Details), new { id = programId });
    }
}