using FitnessTracking.Web;
using FitnessTracking.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseAddress = new Uri("https://localhost:7211/"); // FitnessTracking.Api URL

builder.Services.AddHttpClient<IExercisesService, ExercisesService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IWorkoutProgramsService, WorkoutProgramsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddStandardResilienceHandler();

builder.Services.AddHttpClient<IWorkoutSessionsService, WorkoutSessionsService>(client =>
    client.BaseAddress = apiBaseAddress)
    .AddStandardResilienceHandler();

await builder.Build().RunAsync();
