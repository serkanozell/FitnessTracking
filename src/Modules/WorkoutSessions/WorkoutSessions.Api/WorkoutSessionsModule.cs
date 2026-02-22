using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WorkoutSessions.Api;

public sealed class WorkoutSessionsModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(Application.AssemblyReference).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapEndpointsFromAssembly(typeof(Application.AssemblyReference).Assembly);
    }
}