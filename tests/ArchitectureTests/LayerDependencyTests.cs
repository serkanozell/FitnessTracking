using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace ArchitectureTests;

public class LayerDependencyTests
{
    // Domain assemblies
    private static readonly Assembly ExercisesDomain = typeof(Exercises.Domain.Entity.Exercise).Assembly;
    private static readonly Assembly WorkoutProgramsDomain = typeof(WorkoutPrograms.Domain.Entity.WorkoutProgram).Assembly;
    private static readonly Assembly WorkoutSessionsDomain = typeof(WorkoutSessions.Domain.Entity.WorkoutSession).Assembly;

    // Application assemblies
    private static readonly Assembly ExercisesApplication = typeof(Exercises.Application.AssemblyReference).Assembly;
    private static readonly Assembly WorkoutProgramsApplication = typeof(WorkoutPrograms.Application.AssemblyReference).Assembly;
    private static readonly Assembly WorkoutSessionsApplication = typeof(WorkoutSessions.Application.AssemblyReference).Assembly;

    // Infrastructure assemblies
    private static readonly Assembly ExercisesInfrastructure = typeof(Exercises.Infrastructure.Repositories.ExerciseRepository).Assembly;
    private static readonly Assembly WorkoutProgramsInfrastructure = typeof(WorkoutPrograms.Infrastructure.Repositories.WorkoutProgramRepository).Assembly;
    private static readonly Assembly WorkoutSessionsInfrastructure = typeof(WorkoutSessions.Infrastructure.Repositories.WorkoutSessionRepository).Assembly;

    // ── Domain should NOT depend on Application ──

    [Theory]
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
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
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
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
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
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
    [InlineData("Exercises")]
    [InlineData("WorkoutPrograms")]
    [InlineData("WorkoutSessions")]
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
    public void ExercisesDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(ExercisesDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "WorkoutPrograms.Domain",
                "WorkoutSessions.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void WorkoutProgramsDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(WorkoutProgramsDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Exercises.Domain",
                "WorkoutSessions.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void WorkoutSessionsDomain_ShouldNotDependOn_OtherModuleDomains()
    {
        var result = Types.InAssembly(WorkoutSessionsDomain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Exercises.Domain",
                "WorkoutPrograms.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    private Assembly GetDomainAssembly(string module) => module switch
    {
        "Exercises" => ExercisesDomain,
        "WorkoutPrograms" => WorkoutProgramsDomain,
        "WorkoutSessions" => WorkoutSessionsDomain,
        _ => throw new ArgumentException(module)
    };

    private Assembly GetApplicationAssembly(string module) => module switch
    {
        "Exercises" => ExercisesApplication,
        "WorkoutPrograms" => WorkoutProgramsApplication,
        "WorkoutSessions" => WorkoutSessionsApplication,
        _ => throw new ArgumentException(module)
    };
}
