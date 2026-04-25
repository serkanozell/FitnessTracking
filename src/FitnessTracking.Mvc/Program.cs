using System.Globalization;
using FitnessTracking.Mvc.Middleware;
using FitnessTracking.Mvc.Services;
using FitnessTracking.Mvc.Services.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

// Centralized resilience tuning. The library defaults are
// (AttemptTimeout=10s, 3 retries, TotalRequestTimeout=30s, SamplingDuration=30s).
// 3 retries amplifies one slow call into 30+ seconds with multiple logged
// exceptions; meanwhile setting AttemptTimeout too low (e.g. 5s) causes
// TimeoutRejectedException -> TaskCanceledException -> SocketException chains
// for legitimate but slow API responses. We pick generous timeouts with at
// most one retry. Constraints enforced by the library:
//   AttemptTimeout         <= CircuitBreaker.SamplingDuration
//   TotalRequestTimeout    >= AttemptTimeout * 2
static void ConfigureResilience(HttpStandardResilienceOptions options)
{
    // Per-attempt cap. Generous enough to absorb a slow EF query / cold cache
    // round-trip without canceling a healthy request.
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);

    // Must be >= 2 * AttemptTimeout. Covers attempt + 1 retry + a little slack.
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(90);

    // Single retry only. Keeps log noise low and avoids 30s+ stalls on real
    // failures while still recovering from transient glitches.
    options.Retry.MaxRetryAttempts = 1;

    // Must be >= AttemptTimeout.
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
}

builder.Services.AddControllersWithViews();

// Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthTokenHandler>();

var apiBaseAddress = new Uri(
    builder.Configuration.GetValue<string>("ApiBaseAddress") ?? "https://localhost:7211/");

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
    client.BaseAddress = apiBaseAddress);

builder.Services.AddHttpClient<IExercisesService, ExercisesService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler(ConfigureResilience);

builder.Services.AddHttpClient<IWorkoutProgramsService, WorkoutProgramsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler(ConfigureResilience);

builder.Services.AddHttpClient<IWorkoutSessionsService, WorkoutSessionsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler(ConfigureResilience);

builder.Services.AddHttpClient<IBodyMetricsService, BodyMetricsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler(ConfigureResilience);

builder.Services.AddHttpClient<INutritionService, NutritionService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler(ConfigureResilience);

builder.Services.AddHttpClient<IDashboardService, DashboardService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30)
    })
    .AddStandardResilienceHandler(options =>
    {
        // Dashboard aggregates many backend calls; needs more headroom than the others.
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
        options.Retry.MaxRetryAttempts = 1;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHttpClient<IUserManagementService, UserManagementService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler(ConfigureResilience);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Force invariant culture so decimal values (e.g. 7.5) are parsed correctly
// regardless of the browser/OS locale (Turkish uses comma as decimal separator).
var invariantCulture = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(invariantCulture),
    SupportedCultures = [invariantCulture],
    SupportedUICultures = [invariantCulture]
});

// Swallow client-cancellation exceptions (browser navigation/refresh) so that
// they don't pollute logs or hit UseExceptionHandler. Must run before MVC and
// after routing so we still have route info in the diagnostic log.
app.UseClientDisconnectedHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
