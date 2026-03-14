using FitnessTracking.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApiConfiguration(builder.Configuration)
                .AddJwtAuthentication(builder.Configuration)
                .AddInfrastructure(builder.Configuration);

var modules = builder.Services.AddModules(builder.Configuration);

var app = builder.Build();

app.UseApiMiddleware(modules);

app.Run();