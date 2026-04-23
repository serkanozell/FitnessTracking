using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class BodyMetricsController(IBodyMetricsService bodyMetricsService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var result = await bodyMetricsService.GetPagedAsync(page, pageSize, HttpContext.RequestAborted);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var metric = await bodyMetricsService.GetByIdAsync(id, HttpContext.RequestAborted);
        if (metric is null) return NotFound();
        return View(metric);
    }

    [HttpGet]
    public IActionResult Create() => View(new BodyMetricEditModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BodyMetricEditModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await bodyMetricsService.CreateAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "Body metric recorded.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var metric = await bodyMetricsService.GetByIdAsync(id, HttpContext.RequestAborted);
        if (metric is null) return NotFound();

        var model = new BodyMetricEditModel
        {
            Date = metric.Date,
            Weight = metric.Weight,
            Height = metric.Height,
            BodyFatPercentage = metric.BodyFatPercentage,
            MuscleMass = metric.MuscleMass,
            WaistCircumference = metric.WaistCircumference,
            ChestCircumference = metric.ChestCircumference,
            ArmCircumference = metric.ArmCircumference,
            HipCircumference = metric.HipCircumference,
            ThighCircumference = metric.ThighCircumference,
            NeckCircumference = metric.NeckCircumference,
            ShoulderCircumference = metric.ShoulderCircumference,
            Note = metric.Note
        };
        ViewData["MetricId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, BodyMetricEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["MetricId"] = id;
            return View(model);
        }

        var ok = await bodyMetricsService.UpdateAsync(id, model, HttpContext.RequestAborted);
        if (!ok) return NotFound();

        TempData["Success"] = "Body metric updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await bodyMetricsService.ActivateAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Body metric activated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await bodyMetricsService.DeleteAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "Body metric deleted.";
        return RedirectToAction(nameof(Index));
    }
}
