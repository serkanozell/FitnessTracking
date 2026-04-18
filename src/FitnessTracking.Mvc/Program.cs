using FitnessTracking.Mvc.Services;
using FitnessTracking.Mvc.Services.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

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
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IWorkoutProgramsService, WorkoutProgramsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IWorkoutSessionsService, WorkoutSessionsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IBodyMetricsService, BodyMetricsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<INutritionService, NutritionService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler();

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
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHttpClient<IUserManagementService, UserManagementService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddHttpMessageHandler<AuthTokenHandler>()
    .AddStandardResilienceHandler();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
