using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize(Policy = "Admin")]
public class UsersController(IUserManagementService userService) : Controller
{
    public async Task<IActionResult> Index(Guid? id)
    {
        var roles = await userService.GetAllRolesAsync(HttpContext.RequestAborted);
        ViewData["AllRoles"] = roles;

        UserDto? user = null;
        if (id.HasValue)
        {
            user = await userService.GetUserByIdAsync(id.Value, HttpContext.RequestAborted);
        }
        ViewData["SelectedUser"] = user;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var roles = await userService.GetAllRolesAsync(HttpContext.RequestAborted);
        ViewData["AllRoles"] = roles;
        return View(new CreateUserModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserModel model)
    {
        if (!ModelState.IsValid)
        {
            var roles = await userService.GetAllRolesAsync(HttpContext.RequestAborted);
            ViewData["AllRoles"] = roles;
            return View(model);
        }

        await userService.CreateUserAsync(model, HttpContext.RequestAborted);
        TempData["Success"] = "User created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id, HttpContext.RequestAborted);
        if (user is null) return NotFound();

        var roles = await userService.GetAllRolesAsync(HttpContext.RequestAborted);
        ViewData["AllRoles"] = roles;
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        await userService.ActivateUserAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "User activated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await userService.DeleteUserAsync(id, HttpContext.RequestAborted);
        TempData["Success"] = "User deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(Guid id, Guid roleId)
    {
        await userService.AssignRoleAsync(id, roleId, HttpContext.RequestAborted);
        TempData["Success"] = "Role assigned.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(Guid id, Guid roleId)
    {
        await userService.RemoveRoleAsync(id, roleId, HttpContext.RequestAborted);
        TempData["Success"] = "Role removed.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
