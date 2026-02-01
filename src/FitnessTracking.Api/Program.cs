using BuildingBlocks.Infrastructure.Services;
using Exercises.Infrastructure;
using Microsoft.EntityFrameworkCore;
using WorkoutPrograms.Infrastructure;
using WorkoutSessions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExercisesInfrastructure(builder.Configuration)
                .WorkoutProgramsInfrastructure(builder.Configuration)
                .WorkoutSessionsInfrastructure(builder.Configuration);


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Exercises.Application.AssemblyReference).Assembly,
        typeof(WorkoutPrograms.Application.AssemblyReference).Assembly,
        typeof(WorkoutSessions.Application.AssemblyReference).Assembly);
});

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

app.UseCors("BlazorClient");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
