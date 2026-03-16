using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.Builder;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.Security;
using BuildingBlocks.Infrastructure.Services;
using BuildingBlocks.Web;
using Exercises.Api;
using Exercises.Infrastructure;
using FitnessTracking.Api.ExceptionHandling;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Users.Api;
using Users.Infrastructure;
using WorkoutPrograms.Api;
using WorkoutPrograms.Infrastructure;
using WorkoutSessions.Api;
using WorkoutSessions.Infrastructure;
using BodyMetrics.Api;
using BodyMetrics.Infrastructure;
using Dashboard.Api;

namespace FitnessTracking.Api.Extensions
{
    public static class ProgramExtensions
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenApi();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 5,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }));
            });

            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            services.AddCors(options =>
            {
                options.AddPolicy("BlazorClient", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

            if (jwtOptions is not null && !string.IsNullOrWhiteSpace(jwtOptions.Key))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtOptions.Issuer,
                            ValidAudience = jwtOptions.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                        };
                    });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                });
            }

            services.AddSingleton<ITokenService, TokenService>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBuildingBlocksInfrastructure(configuration);

            services.AddUsersInfrastructure(configuration)
                    .AddExercisesInfrastructure(configuration)
                    .WorkoutProgramsInfrastructure(configuration)
                    .WorkoutSessionsInfrastructure(configuration)
                    .AddBodyMetricsInfrastructure(configuration);

            return services;
        }

        public static IModule[] AddModules(this IServiceCollection services, IConfiguration configuration)
        {
            IModule[] modules =
            [
                new UsersModule(),
                new ExercisesModule(),
                new WorkoutProgramsModule(),
                new WorkoutSessionsModule(),
                new BodyMetricsModule(),
                new DashboardModule()
            ];

            var moduleAssemblies = modules.Select(m => m.ApplicationAssembly).ToArray();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(moduleAssemblies);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            });

            foreach (var assembly in moduleAssemblies)
                services.AddValidatorsFromAssembly(assembly);

            foreach (var module in modules)
            {
                module.Register(services, configuration);
            }

            return modules;
        }

        public static WebApplication UseApiMiddleware(this WebApplication app, IModule[] modules)
        {
            app.UseExceptionHandler();

            ApiVersionSet versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var v1 = app.MapGroup("/api/v{version:apiVersion}")
                        .WithApiVersionSet(versionSet)
                        .MapToApiVersion(new ApiVersion(1, 0))
                        .RequireAuthorization();

            foreach (var module in modules)
            {
                module.MapEndpoints(v1);
            }

            app.UseHttpsRedirection();

            app.UseCors("BlazorClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("FitnessTracking API");
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            return app;
        }
    }
}