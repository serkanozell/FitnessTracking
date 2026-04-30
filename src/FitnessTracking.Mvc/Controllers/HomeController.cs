using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class HomeController(IDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var ct = HttpContext.RequestAborted;

        // Independent dashboard lookups; run in parallel.
        var dashboardTask = dashboardService.GetDashboardAsync(ct);
        var weightTrendTask = dashboardService.GetWeightTrendAsync(90, ct);

        await Task.WhenAll(dashboardTask, weightTrendTask);

        ViewData["WeightTrend"] = weightTrendTask.Result;

        return View(dashboardTask.Result);
    }
}
