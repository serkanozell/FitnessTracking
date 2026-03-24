using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

public class AccountController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await authService.LoginAsync(model);

        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, result.Email),
            new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
            new(AuthTokenHandler.JwtClaimType, result.Token)
        };

        // Parse roles from JWT
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Token);
            var roleClaims = jwt.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role");
            claims.AddRange(roleClaims.Select(c => new Claim(ClaimTypes.Role, c.Value)));
        }
        catch
        {
            // If JWT parsing fails, continue without role claims
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        return LocalRedirect(returnUrl ?? "/");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await authService.RegisterAsync(model);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Registration failed. Email may already be in use.");
            return View(model);
        }

        TempData["Success"] = "Account created successfully. Please sign in.";
        return RedirectToAction(nameof(Login));
    }
}
