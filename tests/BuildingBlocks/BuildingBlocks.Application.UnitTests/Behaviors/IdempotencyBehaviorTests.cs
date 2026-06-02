using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Abstractions.Idempotency;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Application.Results;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace BuildingBlocks.Application.UnitTests.Behaviors;

public class IdempotencyBehaviorTests
{
    private sealed record TestIdempotentCommand(string? IdempotencyKey)
        : ICommand<Result<string>>, IIdempotentCommand;

    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly IOptions<IdempotencyOptions> _options =
        Options.Create(new IdempotencyOptions { ExpirationMinutes = 60 });

    [Fact]
    public async Task Handle_ShouldBypassCache_WhenIdempotencyKeyIsNull()
    {
        var handlerResult = Result<string>.Success("created");
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next(Arg.Any<CancellationToken>()).Returns(handlerResult);

        var sut = new IdempotencyBehavior<TestIdempotentCommand, Result<string>>(_cacheService, _options);

        var result = await sut.Handle(new TestIdempotentCommand(null), next, CancellationToken.None);

        result.Should().Be(handlerResult);
        await _cacheService.DidNotReceive().GetAsync<Result<string>>(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cacheService.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<Result<string>>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReplayCachedResponse_WhenKeyAlreadyProcessed()
    {
        var cached = Result<string>.Success("original");
        _cacheService.GetAsync<Result<string>>("idempotency:abc", Arg.Any<CancellationToken>()).Returns(cached);

        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        var sut = new IdempotencyBehavior<TestIdempotentCommand, Result<string>>(_cacheService, _options);

        var result = await sut.Handle(new TestIdempotentCommand("abc"), next, CancellationToken.None);

        result.Should().Be(cached);
        await next.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldStoreSuccessfulResponse_WhenCacheMiss()
    {
        _cacheService.GetAsync<Result<string>>("idempotency:abc", Arg.Any<CancellationToken>())
            .Returns((Result<string>?)null);

        var handlerResult = Result<string>.Success("created");
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next(Arg.Any<CancellationToken>()).Returns(handlerResult);

        var sut = new IdempotencyBehavior<TestIdempotentCommand, Result<string>>(_cacheService, _options);

        var result = await sut.Handle(new TestIdempotentCommand("abc"), next, CancellationToken.None);

        result.Should().Be(handlerResult);
        await _cacheService.Received(1).SetAsync(
            "idempotency:abc",
            handlerResult,
            TimeSpan.FromMinutes(60),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldNotStoreResponse_WhenResultIsFailure()
    {
        _cacheService.GetAsync<Result<string>>("idempotency:abc", Arg.Any<CancellationToken>())
            .Returns((Result<string>?)null);

        var handlerResult = Result<string>.Failure(Error.Conflict("duplicate"));
        var next = Substitute.For<RequestHandlerDelegate<Result<string>>>();
        next(Arg.Any<CancellationToken>()).Returns(handlerResult);

        var sut = new IdempotencyBehavior<TestIdempotentCommand, Result<string>>(_cacheService, _options);

        var result = await sut.Handle(new TestIdempotentCommand("abc"), next, CancellationToken.None);

        result.Should().Be(handlerResult);
        await _cacheService.DidNotReceive().SetAsync(
            Arg.Any<string>(),
            Arg.Any<Result<string>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }
}
