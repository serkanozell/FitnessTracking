using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class ExercisesController(IExercisesService exercisesService, IWebHostEnvironment env) : Controller
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

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

        await ProcessImageUploadAsync(model);
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
            Description = exercise.Description,
            ImageUrl = exercise.ImageUrl,
            VideoUrl = exercise.VideoUrl
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

        var existing = await exercisesService.GetByIdAsync(id, HttpContext.RequestAborted);

        if (model.RemoveImage && !string.IsNullOrEmpty(existing?.ImageUrl))
        {
            DeleteImageFile(existing.ImageUrl);
            model.ImageUrl = null;
        }
        else if (model.ImageFile is not null)
        {
            if (!string.IsNullOrEmpty(existing?.ImageUrl))
                DeleteImageFile(existing.ImageUrl);

            await ProcessImageUploadAsync(model);
            if (!ModelState.IsValid)
            {
                ViewData["ExerciseId"] = id;
                return View(model);
            }
        }
        else
        {
            model.ImageUrl = existing?.ImageUrl;
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

    private async Task ProcessImageUploadAsync(ExerciseEditModel model)
    {
        if (model.ImageFile is null || model.ImageFile.Length == 0)
            return;

        if (model.ImageFile.Length > MaxFileSize)
        {
            ModelState.AddModelError(nameof(model.ImageFile), "Image file must be less than 5 MB.");
            return;
        }

        var ext = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            ModelState.AddModelError(nameof(model.ImageFile), "Only JPG, PNG, WebP and GIF files are allowed.");
            return;
        }

        var uploadsDir = Path.Combine(env.WebRootPath, "uploads", "exercises");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await model.ImageFile.CopyToAsync(stream);

        model.ImageUrl = $"/uploads/exercises/{fileName}";
    }

    private void DeleteImageFile(string imageUrl)
    {
        if (!imageUrl.StartsWith("/uploads/"))
            return;

        var filePath = Path.Combine(env.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
    }
}
