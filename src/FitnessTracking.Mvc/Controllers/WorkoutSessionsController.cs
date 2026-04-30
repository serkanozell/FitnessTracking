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
    public async Task<IActionResult> Index(Guid? programId = null, int page = 1, int pageSize = 10)
    {
        var ct = HttpContext.RequestAborted;
        try
        {
            var programsTask = programsService.GetPagedAsync(1, 100, ct);
            var exercisesTask = exercisesService.GetPagedAsync(1, 100, ct);

            if (programId.HasValue)
            {
                var sessionsTask = sessionsService.GetPagedAsync(programId, page, pageSize, ct);
                await Task.WhenAll(sessionsTask, programsTask, exercisesTask);

                ViewData["Programs"] = programsTask.Result.Items;
                ViewData["AllExercises"] = exercisesTask.Result.Items;
                ViewData["SelectedProgramId"] = programId.Value;

                return View(sessionsTask.Result);
            }

            await Task.WhenAll(programsTask, exercisesTask);

            ViewData["Programs"] = programsTask.Result.Items;
            ViewData["AllExercises"] = exercisesTask.Result.Items;

            return View(new PagedResult<Models.WorkoutSessionDto>());
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
        try
        {
            var programs = await programsService.GetPagedAsync(1, 100, ct);
            ViewData["Programs"] = programs.Items;
            ViewData["ProgramSplits"] = await LoadProgramSplitsMapAsync(programs.Items, ct);
            return View(new WorkoutSessionEditModel());
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new EmptyResult();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutSessionEditModel model)
    {
        var ct = HttpContext.RequestAborted;
        try
        {
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
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new EmptyResult();
        }
    }

    private async Task<Dictionary<Guid, IReadOnlyList<WorkoutProgramSplitDto>>> LoadProgramSplitsMapAsync(
        IEnumerable<WorkoutProgramDto> programs, CancellationToken ct)
    {
        // Fan out the per-program split lookups in parallel rather than awaiting
        // them sequentially (which previously meant up to 100 round-trips back-to-back).
        var programList = programs as IReadOnlyCollection<WorkoutProgramDto> ?? programs.ToList();
        var tasks = programList
            .Select(async p => (p.Id, Splits: await programsService.GetSplitsAsync(p.Id, ct)))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        var map = new Dictionary<Guid, IReadOnlyList<WorkoutProgramSplitDto>>(results.Length);
        foreach (var (id, splits) in results)
            map[id] = splits;
        return map;
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var ct = HttpContext.RequestAborted;
        try
        {
            var view = await sessionsService.GetDetailViewAsync(id, ct);
            if (view is null) return NotFound();
            return View(view);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new EmptyResult();
        }
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
    public async Task<IActionResult> UpdateDate(Guid id, DateTime date)
    {
        var ct = HttpContext.RequestAborted;
        try
        {
            var model = new WorkoutSessionEditModel { Date = date };
            var success = await sessionsService.UpdateAsync(id, model, ct);
            if (!success) return NotFound();
            TempData["Success"] = "Session date updated.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new EmptyResult();
        }
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
