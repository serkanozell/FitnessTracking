using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize(Policy = "Admin")]
public class RolesController(IUserManagementService userService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var roles = await userService.GetAllRolesAsync(HttpContext.RequestAborted);
        return View(roles);
    }

    [HttpGet]
    public IActionResult Create() => View(new RoleEditModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleEditModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await userService.CreateRoleAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "Role created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var roles = await userService.GetAllRolesAsync(HttpContext.RequestAborted);
        var role = roles.FirstOrDefault(r => r.Id == id);
        if (role is null) return NotFound();

        ViewData["RoleId"] = id;
        return View(new RoleEditModel { Name = role.Name, Description = role.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, RoleEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["RoleId"] = id;
            return View(model);
        }

        var ok = await userService.UpdateRoleAsync(id, model, HttpContext.RequestAborted);
        if (!ok) return NotFound();

        TempData["Success"] = "Role updated.";
        return RedirectToAction(nameof(Index));
    }
}
