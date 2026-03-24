using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class HomeController(IDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var dashboard = await dashboardService.GetDashboardAsync(HttpContext.RequestAborted);
        var weightTrend = await dashboardService.GetWeightTrendAsync(90, HttpContext.RequestAborted);

        ViewData["WeightTrend"] = weightTrend;

        return View(dashboard);
    }
}
