using System.Reflection;
using FluentAssertions;
using FluentValidation;
using MediatR;
using NetArchTest.Rules;
using Xunit;

namespace ArchitectureTests;

public class NamingConventionTests
{
    private static readonly Assembly[] ApplicationAssemblies =
    [
        typeof(Exercises.Application.AssemblyReference).Assembly,
        typeof(WorkoutPrograms.Application.AssemblyReference).Assembly,
        typeof(WorkoutSessions.Application.AssemblyReference).Assembly
    ];

    [Fact]
    public void CommandHandlers_ShouldEndWith_CommandHandler()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryHandlers_ShouldEndWith_QueryHandler()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .And()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validators_ShouldEndWith_Validator()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .HaveNameEndingWith("Validator")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            because: FailingTypeNames(result));
    }

    [Fact]
    public void Commands_ShouldEndWith_Command()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .ImplementInterface(typeof(IRequest<>))
            .And()
            .HaveNameEndingWith("Command")
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Queries_ShouldEndWith_Query()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .ImplementInterface(typeof(IRequest<>))
            .And()
            .HaveNameEndingWith("Query")
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Endpoints_ShouldEndWith_Endpoint()
    {
        var result = Types.InAssemblies(ApplicationAssemblies)
            .That()
            .HaveNameEndingWith("Endpoint")
            .Should()
            .HaveNameEndingWith("Endpoint")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    private static string FailingTypeNames(NetArchTest.Rules.TestResult result) =>
        result.FailingTypeNames is null
            ? string.Empty
            : string.Join(", ", result.FailingTypeNames);
}
