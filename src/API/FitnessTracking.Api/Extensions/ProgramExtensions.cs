using System.Security.Claims;
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
using Microsoft.Extensions.Options;
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
using Nutrition.Api;
using Nutrition.Infrastructure;
using FitnessTracking.Api.Configuration;

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

            services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Rules are resolved per request from IOptionsMonitor so configuration changes
                // (and test overrides via PostConfigure) take effect without recompiling the limiter.
                // Coarse per-IP backstop applied to every request.
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    CreateFixedWindow(GetClientIp(context), GetRules(context).Global));

                // Strict per-IP policy for auth endpoints (brute-force / enumeration protection).
                options.AddPolicy(RateLimitPolicies.Authentication, context =>
                    CreateFixedWindow(GetClientIp(context), GetRules(context).Authentication));

                // Generous per-user policy for dashboard/analytics endpoints (high fan-out per page load).
                // Falls back to IP when the request is unauthenticated.
                options.AddPolicy(RateLimitPolicies.Dashboard, context =>
                    CreateFixedWindow(GetUserPartitionKey(context), GetRules(context).Dashboard));
            });

            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            services.AddCors(options =>
            {
                options.AddPolicy("WebClient", policy =>
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
                    .AddWorkoutProgramsInfrastructure(configuration)
                    .AddWorkoutSessionsInfrastructure(configuration)
                    .AddBodyMetricsInfrastructure(configuration)
                    .AddNutritionInfrastructure(configuration);

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
                new NutritionModule(),
                new DashboardModule()
            ];

            var moduleAssemblies = modules.Select(m => m.ApplicationAssembly).ToArray();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(moduleAssemblies);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(IdempotencyBehavior<,>));
                cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
                cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
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

            app.UseCors("WebClient");

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

        private static RateLimitPartition<string> CreateFixedWindow(string partitionKey, RateLimitRule rule) =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = rule.PermitLimit,
                    Window = rule.Window,
                    QueueLimit = rule.QueueLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });

        // Resolves the current rules from DI per request so configuration reloads and test overrides
        // are honored, instead of capturing a snapshot at limiter-registration time.
        private static RateLimitingOptions GetRules(HttpContext context) =>
            context.RequestServices.GetRequiredService<IOptionsMonitor<RateLimitingOptions>>().CurrentValue;

        private static string GetClientIp(HttpContext context) =>
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Partition authenticated requests by user id so one user's burst can't exhaust the limit
        // for other users sharing the same IP (NAT/proxy); fall back to IP when anonymous.
        private static string GetUserPartitionKey(HttpContext context)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userId) ? $"ip:{GetClientIp(context)}" : $"user:{userId}";
        }
    }
}