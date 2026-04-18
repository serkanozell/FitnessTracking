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
        var ct = HttpContext.RequestAborted;
        try
        {
            var sessionsTask = sessionsService.GetPagedAsync(page, pageSize, ct);
            var programsTask = programsService.GetPagedAsync(1, 100, ct);
            var exercisesTask = exercisesService.GetPagedAsync(1, 100, ct);

            await Task.WhenAll(sessionsTask, programsTask, exercisesTask);

            ViewData["Programs"] = programsTask.Result.Items;
            ViewData["AllExercises"] = exercisesTask.Result.Items;

            return View(sessionsTask.Result);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Client disconnected before the response could be produced.
            return new EmptyResult();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var ct = HttpContext.RequestAborted;
        var programs = await programsService.GetPagedAsync(1, 100, ct);
        ViewData["Programs"] = programs.Items;
        ViewData["ProgramSplits"] = await LoadProgramSplitsMapAsync(programs.Items, ct);
        return View(new WorkoutSessionEditModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutSessionEditModel model)
    {
        var ct = HttpContext.RequestAborted;
        if (!ModelState.IsValid)
        {
            var programs = await programsService.GetPagedAsync(1, 100, ct);
            ViewData["Programs"] = programs.Items;
            ViewData["ProgramSplits"] = await LoadProgramSplitsMapAsync(programs.Items, ct);
            return View(model);
        }

        var id = await sessionsService.CreateAsync(model, ct);
        TempData["Success"] = "Session created.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<Dictionary<Guid, IReadOnlyList<WorkoutProgramSplitDto>>> LoadProgramSplitsMapAsync(
        IEnumerable<WorkoutProgramDto> programs, CancellationToken ct)
    {
        var map = new Dictionary<Guid, IReadOnlyList<WorkoutProgramSplitDto>>();
        foreach (var p in programs)
        {
            map[p.Id] = await programsService.GetSplitsAsync(p.Id, ct);
        }
        return map;
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var ct = HttpContext.RequestAborted;
        var session = await sessionsService.GetByIdAsync(id, ct);
        if (session is null) return NotFound();

        var allExercises = await exercisesService.GetPagedAsync(1, 100, ct);
        var programs = await programsService.GetPagedAsync(1, 100, ct);

        // Load splits and their exercises for the program linked to this session,
        // so the user can pick a split and only see exercises that belong to it.
        var splits = await programsService.GetSplitsAsync(session.WorkoutProgramId, ct);
        var splitExercises = new Dictionary<Guid, IReadOnlyList<WorkoutProgramExerciseDto>>();
        foreach (var split in splits)
        {
            var ex = await programsService.GetSplitExercisesAsync(session.WorkoutProgramId, split.Id, ct);
            splitExercises[split.Id] = ex;
        }

        ViewData["AllExercises"] = allExercises.Items;
        ViewData["Programs"] = programs.Items;
        ViewData["Splits"] = splits;
        ViewData["SplitExercises"] = splitExercises;
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
    public async Task<IActionResult> UpdateExercise(Guid id, Guid workoutExerciseId, Guid exerciseId, int setNumber, decimal weight, int reps, string? returnTo, int? page, int? pageSize)
    {
        var model = new WorkoutExerciseEditModel
        {
            ExerciseId = exerciseId,
            SetNumber = setNumber,
            Weight = weight,
            Reps = reps
        };
        await sessionsService.UpdateWorkoutExerciseAsync(id, workoutExerciseId, model, HttpContext.RequestAborted);
        TempData["Success"] = "Set updated.";
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
