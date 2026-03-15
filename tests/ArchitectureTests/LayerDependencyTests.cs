using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace ArchitectureTests;

public class LayerDependencyTests
{
    // Domain assemblies
    private static readonly Assembly UsersDomain = typeof(Users.Domain.Entity.User).Assembly;
    private static readonly Assembly ExercisesDomain = typeof(Exercises.Domain.Entity.Exercise).Assembly;
    private static readonly Assembly WorkoutProgramsDomain = typeof(WorkoutPrograms.Domain.Entity.WorkoutProgram).Assembly;
    private static readonly Assembly WorkoutSessionsDomain = typeof(WorkoutSessions.Domain.Entity.WorkoutSession).Assembly;
    private static readonly Assembly BodyMetricsDomain = typeof(BodyMetrics.Domain.Entity.BodyMetric).Assembly;

    // Application assemblies
    private static readonly Assembly UsersApplication = typeof(Users.Application.AssemblyReference).Assembly;
    private static readonly Assembly ExercisesApplication = typeof(Exercises.Application.AssemblyReference).Assembly;
    private static readonly Assembly WorkoutProgramsApplication = typeof(WorkoutPrograms.Application.AssemblyReference).Assembly;
    private static readonly Assembly WorkoutSessionsApplication = typeof(WorkoutSessions.Application.AssemblyReference).Assembly;
    private static readonly Assembly BodyMetricsApplication = typeof(BodyMetrics.Application.AssemblyReference).Assembly;

    // Infrastructure assemblies
    private static readonly Assembly UsersInfrastructure = typeof(Users.Infrastructure.Repositories.UserRepository).Assembly;
    private static readonly Assembly ExercisesInfrastructure = typeof(Exercises.Infrastructure.Repositories.ExerciseRepository).Assembly;
    private static readonly Assembly WorkoutProgramsInfrastructure = typeof(WorkoutPrograms.Infrastructure.Repositories.WorkoutProgramRepository).Assembly;
    private static readonly Assembly WorkoutSessionsInfrastructure = typeof(WorkoutSessions.Infrastructure.Repositories.WorkoutSessionRepository).Assembly;
    private static readonly Assembly BodyMetricsInfrastructure = typeof(BodyMetrics.Infrastructure.Repositories.BodyMetricRepository).Assembly;

    // ── Domain should NOT depend on Application ──

    [Theory]
    [InlineData("Users")]
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
    [InlineData("BodyMetrics")]
    public void Domain_ShouldNotDependOn_Application(string module)
    {
        var domainAssembly = GetDomainAssembly(module);

        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn($"{module}.Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{module}.Domain should not depend on {module}.Application");
    }

    // ── Domain should NOT depend on Infrastructure ──

    [Theory]
    [InlineData("Users")]
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
    [InlineData("BodyMetrics")]
    public void Domain_ShouldNotDependOn_Infrastructure(string module)
    {
        var domainAssembly = GetDomainAssembly(module);

        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn($"{module}.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{module}.Domain should not depend on {module}.Infrastructure");
    }

    // ── Application should NOT depend on Infrastructure ──

    [Theory]
    [InlineData("Users")]
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
    [InlineData("BodyMetrics")]
    public void Application_ShouldNotDependOn_Infrastructure(string module)
    {
        var appAssembly = GetApplicationAssembly(module);

        var result = Types.InAssembly(appAssembly)
            .ShouldNot()
            .HaveDependencyOn($"{module}.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{module}.Application should not depend on {module}.Infrastructure");
    }

    // ── Domain should NOT depend on EF Core ──

    [Theory]
    [InlineData("Users")]
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
    [InlineData("BodyMetrics")]
    public void Domain_ShouldNotDependOn_EntityFramework(string module)
    {
        var domainAssembly = GetDomainAssembly(module);

        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{module}.Domain should not depend on EntityFrameworkCore");
    }

    // ── Module Isolation: Exercises Domain should NOT depend on other module domains ──

    [Fact]
    public void UsersDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(UsersDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Exercises.Domain",
                "WorkoutPrograms.Domain",
                "WorkoutSessions.Domain",
                "BodyMetrics.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ExercisesDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(ExercisesDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Users.Domain",
                "WorkoutPrograms.Domain",
                "WorkoutSessions.Domain",
                "BodyMetrics.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void WorkoutProgramsDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(WorkoutProgramsDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Users.Domain",
                "Exercises.Domain",
                "WorkoutSessions.Domain",
                "BodyMetrics.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void WorkoutSessionsDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(WorkoutSessionsDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Users.Domain",
                "Exercises.Domain",
                "WorkoutPrograms.Domain",
                "BodyMetrics.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void BodyMetricsDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(BodyMetricsDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Users.Domain",
                "Exercises.Domain",
                "WorkoutPrograms.Domain",
                "WorkoutSessions.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    private Assembly GetDomainAssembly(string module) => module switch
    {
        "Users" => UsersDomain,
        "Exercises" => ExercisesDomain,
        "WorkoutPrograms" => WorkoutProgramsDomain,
        "WorkoutSessions" => WorkoutSessionsDomain,
        "BodyMetrics" => BodyMetricsDomain,
        _ => throw new ArgumentException(module)
    };

    private Assembly GetApplicationAssembly(string module) => module switch
    {
        "Users" => UsersApplication,
        "Exercises" => ExercisesApplication,
        "WorkoutPrograms" => WorkoutProgramsApplication,
        "WorkoutSessions" => WorkoutSessionsApplication,
        "BodyMetrics" => BodyMetricsApplication,
        _ => throw new ArgumentException(module)
    };
}
