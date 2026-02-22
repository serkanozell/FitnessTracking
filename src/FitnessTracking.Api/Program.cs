using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.Services;
using BuildingBlocks.Web;
using Exercises.Api;
using Exercises.Infrastructure;
using FitnessTracking.Api.ExceptionHandling;
using FluentValidation;
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

builder.Services.AddBuildingBlocksInfrastructure(builder.Configuration);

builder.Services.AddExercisesInfrastructure(builder.Configuration)
                .WorkoutProgramsInfrastructure(builder.Configuration)
                .WorkoutSessionsInfrastructure(builder.Configuration);


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Modülleri yükle
IModule[] modules =
[
    new ExercisesModule(),
    new WorkoutProgramsModule(),
    new WorkoutSessionsModule()
];

// MediatR: handler'lar + validator'lar modül assembly'lerinden, behavior'lar tek sefer
var moduleAssemblies = modules.Select(m => m.ApplicationAssembly).ToArray();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(moduleAssemblies);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
});

foreach (var assembly in moduleAssemblies)
    builder.Services.AddValidatorsFromAssembly(assembly);

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

app.UseExceptionHandler();

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
