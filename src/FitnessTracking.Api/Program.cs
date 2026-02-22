using BuildingBlocks.Infrastructure.Services;
using BuildingBlocks.Web;
using Exercises.Api;
using Exercises.Infrastructure;
using Serilog;
using WorkoutPrograms.Api;
using WorkoutPrograms.Infrastructure;
using WorkoutSessions.Api;
using WorkoutSessions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddBuildingBlocksInfrastructure();

builder.Services.AddExercisesInfrastructure(builder.Configuration)
                .WorkoutProgramsInfrastructure(builder.Configuration)
                .WorkoutSessionsInfrastructure(builder.Configuration);


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Modülleri yükle
IModule[] modules =
{
    new ExercisesModule(),
    new WorkoutProgramsModule(),
    new WorkoutSessionsModule()
};

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7073") // FitnessTracking.Web dev URL’i
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ileride ayrý yapý kurulacak
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            Title = "An error occurred",
            Status = 500
        });
    });
});

// Endpointleri map et
foreach (var module in modules)
{
    module.MapEndpoints(app);
}

app.UseHttpsRedirection();

app.UseCors("BlazorClient");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseAuthorization();

app.Run();
