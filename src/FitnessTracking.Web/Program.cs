using FitnessTracking.Web;
using FitnessTracking.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7211/") // FitnessTracking.Api URL
    });

builder.Services.AddScoped<ExercisesApiClient>();
builder.Services.AddScoped<IWorkoutProgramsService, WorkoutProgramsService>();
builder.Services.AddScoped<WorkoutSessionsApiClient>();

await builder.Build().RunAsync();
