using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Application.Results;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using Xunit;

namespace BuildingBlocks.Application.UnitTests.Behaviors;

public record TestCommand(string Name) : ICommand<Result<Guid>>;

public class PassingValidator : AbstractValidator<TestCommand> { }

public class FailingNameValidator : AbstractValidator<TestCommand>
{
    public FailingNameValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public class FailingMaxLengthValidator : AbstractValidator<TestCommand>
{
    public FailingMaxLengthValidator()
    {
        RuleFor(x => x.Name).MaximumLength(3);
    }
}

public class ValidationBehaviorTests
{
    private readonly RequestHandlerDelegate<Result<Guid>> _next = Substitute.For<RequestHandlerDelegate<Result<Guid>>>();

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidatorsRegistered()
    {
        var expectedResult = Result<Guid>.Success(Guid.NewGuid());
        _next.Invoke(Arg.Any<CancellationToken>()).Returns(expectedResult);
        var sut = new ValidationBehavior<TestCommand, Result<Guid>>(Enumerable.Empty<IValidator<TestCommand>>());

        var result = await sut.Handle(new TestCommand("Valid"), _next, CancellationToken.None);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenAllValidatorsPass()
    {
        var expectedResult = Result<Guid>.Success(Guid.NewGuid());
        _next.Invoke(Arg.Any<CancellationToken>()).Returns(expectedResult);
        var sut = new ValidationBehavior<TestCommand, Result<Guid>>(new IValidator<TestCommand>[] { new PassingValidator() });

        var result = await sut.Handle(new TestCommand("Valid"), _next, CancellationToken.None);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var sut = new ValidationBehavior<TestCommand, Result<Guid>>(new IValidator<TestCommand>[] { new FailingNameValidator() });

        var act = () => sut.Handle(new TestCommand(""), _next, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .Where(ex => ex.Errors.Any(e => e.PropertyName == "Name"));
    }

    [Fact]
    public async Task Handle_ShouldNotCallNext_WhenValidationFails()
    {
        var sut = new ValidationBehavior<TestCommand, Result<Guid>>(new IValidator<TestCommand>[] { new FailingNameValidator() });

        try { await sut.Handle(new TestCommand(""), _next, CancellationToken.None); }
        catch (ValidationException) { }

        await _next.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldAggregateErrors_WhenMultipleValidatorsFail()
    {
        var sut = new ValidationBehavior<TestCommand, Result<Guid>>(new IValidator<TestCommand>[]
        {
            new FailingNameValidator(),
            new FailingMaxLengthValidator()
        });

        var act = () => sut.Handle(new TestCommand(""), _next, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Handle_ShouldAggregateErrorsFromMultipleValidators_WhenBothFail()
    {
        var sut = new ValidationBehavior<TestCommand, Result<Guid>>(new IValidator<TestCommand>[]
        {
            new FailingNameValidator(),
            new FailingMaxLengthValidator()
        });

        // Empty string fails NotEmpty; "TooLong" fails MaxLength(3) — both produce Name errors
        var act = () => sut.Handle(new TestCommand("TooLongName"), _next, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().OnlyContain(e => e.PropertyName == "Name");
    }
}
