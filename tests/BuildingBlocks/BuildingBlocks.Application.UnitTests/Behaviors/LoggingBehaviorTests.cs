using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Results;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BuildingBlocks.Application.UnitTests.Behaviors;

public class LoggingBehaviorTests
{
    private sealed record TestRequest(string Data) : IRequest<Result<string>>;

    private readonly RequestHandlerDelegate<Result<string>> _next = Substitute.For<RequestHandlerDelegate<Result<string>>>();

    [Fact]
    public async Task Handle_ShouldCallNextAndReturnResponse()
    {
        var expectedResult = Result<string>.Success("response");
        _next.Invoke().Returns(expectedResult);

        var sut = new LoggingBehavior<TestRequest, Result<string>>();
        var request = new TestRequest("test");

        var result = await sut.Handle(request, _next, CancellationToken.None);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task Handle_ShouldInvokeNextExactlyOnce()
    {
        _next.Invoke().Returns(Result<string>.Success("ok"));

        var sut = new LoggingBehavior<TestRequest, Result<string>>();

        await sut.Handle(new TestRequest("test"), _next, CancellationToken.None);

        await _next.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenNextThrows()
    {
        _next.Invoke().ThrowsAsync(new InvalidOperationException("handler error"));

        var sut = new LoggingBehavior<TestRequest, Result<string>>();

        var act = () => sut.Handle(new TestRequest("test"), _next, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("handler error");
    }
}
