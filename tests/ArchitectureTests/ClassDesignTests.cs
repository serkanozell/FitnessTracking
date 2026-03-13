using BuildingBlocks.Domain.Abstractions;
using System.Reflection;
using FluentAssertions;
using MediatR;
using NetArchTest.Rules;
using Xunit;

namespace ArchitectureTests;

public class ClassDesignTests
{
    private static readonly Assembly[] ApplicationAssemblies =
    [
        typeof(Exercises.Application.AssemblyReference).Assembly,
        typeof(WorkoutPrograms.Application.AssemblyReference).Assembly,
        typeof(WorkoutSessions.Application.AssemblyReference).Assembly
    ];

    private static readonly Assembly[] InfrastructureAssemblies =
    [
        typeof(Exercises.Infrastructure.Repositories.ExerciseRepository).Assembly,
        typeof(WorkoutPrograms.Infrastructure.Repositories.WorkoutProgramRepository).Assembly,
        typeof(WorkoutSessions.Infrastructure.Repositories.WorkoutSessionRepository).Assembly
    ];

    [Fact]
    public void Handlers_ShouldBeSealed()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All handlers should be sealed. Failing: {FailingNames(result)}");
    }

    [Fact]
    public void Validators_ShouldBeSealed()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .Inherit(typeof(FluentValidation.AbstractValidator<>))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All validators should be sealed. Failing: {FailingNames(result)}");
    }

    [Fact]
    public void DbContexts_ShouldBeSealed()
    {
        var result = Types.InAssemblies(InfrastructureAssemblies)
            .That()
            .Inherit(typeof(Microsoft.EntityFrameworkCore.DbContext))
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"All DbContexts should be sealed. Failing: {FailingNames(result)}");
    }

    [Fact]
    public void DomainEntities_ShouldNotBePublicSetters()
    {
        var domainAssemblies = new[]
        {
            typeof(Exercises.Domain.Entity.Exercise).Assembly,
            typeof(WorkoutPrograms.Domain.Entity.WorkoutProgram).Assembly,
            typeof(WorkoutSessions.Domain.Entity.WorkoutSession).Assembly
        };

        foreach (var assembly in domainAssemblies)
        {
            var entityTypes = Types.InAssembly(assembly)
                .That()
                .Inherit(typeof(Entity<>))
                .GetTypes();

            foreach (var type in entityTypes)
            {
                var nameProp = type.GetProperty("Name");
                if (nameProp != null)
                {
                    var setter = nameProp.GetSetMethod(true);
                    if (setter != null)
                    {
                        setter.IsPublic.Should().BeFalse(
                            $"{type.Name}.Name should not have a public setter");
                    }
                }
            }
        }
    }

    [Fact]
    public void Handlers_ShouldNotBePublic()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .AreNotAbstract()
            .And()
            .DoNotHaveNameEndingWith("EventHandler")
            .And()
            .DoNotHaveNameEndingWith("InvalidationHandler")
            .ShouldNot()
            .BePublic()
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: $"Command/Query handlers should be internal. Failing: {FailingNames(result)}");
    }

    private static string FailingNames(NetArchTest.Rules.TestResult result) =>
        result.FailingTypeNames is null
            ? string.Empty
            : string.Join(", ", result.FailingTypeNames);
}
