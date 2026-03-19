using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.CQRS;
using BuildingBlocks.Application.Results;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace BuildingBlocks.Application.UnitTests.Behaviors;

public class CacheInvalidationBehaviorTests
{
    private sealed record TestInvalidatingCommand : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => ["items:123", "items:123:children"];
        public string[] CachePrefixesToInvalidate => ["items:all"];
    }

    private sealed record TestNoKeysCommand : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [];
        public string[] CachePrefixesToInvalidate => [];
    }

    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();

    [Fact]
    public async Task Handle_ShouldInvalidateCache_WhenCommandSucceeds()
    {
        var next = Substitute.For<RequestHandlerDelegate<Result<Guid>>>();
        next(Arg.Any<CancellationToken>()).Returns(Result<Guid>.Success(Guid.NewGuid()));

        var sut = new CacheInvalidationBehavior<TestInvalidatingCommand, Result<Guid>>(_cacheService);
        var command = new TestInvalidatingCommand();

        await sut.Handle(command, next, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync("items:123", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync("items:123:children", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("items:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldNotInvalidateCache_WhenCommandFails()
    {
        var next = Substitute.For<RequestHandlerDelegate<Result<Guid>>>();
        next(Arg.Any<CancellationToken>()).Returns(Result<Guid>.Failure(new Error("Test.Error", "fail")));

        var sut = new CacheInvalidationBehavior<TestInvalidatingCommand, Result<Guid>>(_cacheService);
        var command = new TestInvalidatingCommand();

        await sut.Handle(command, next, CancellationToken.None);

        await _cacheService.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cacheService.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnResponse_Regardless()
    {
        var expectedId = Guid.NewGuid();
        var next = Substitute.For<RequestHandlerDelegate<Result<Guid>>>();
        next(Arg.Any<CancellationToken>()).Returns(Result<Guid>.Success(expectedId));

        var sut = new CacheInvalidationBehavior<TestInvalidatingCommand, Result<Guid>>(_cacheService);

        var result = await sut.Handle(new TestInvalidatingCommand(), next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedId);
    }

    [Fact]
    public async Task Handle_ShouldNotCallCache_WhenNoKeysOrPrefixes()
    {
        var next = Substitute.For<RequestHandlerDelegate<Result<bool>>>();
        next(Arg.Any<CancellationToken>()).Returns(Result<bool>.Success(true));

        var sut = new CacheInvalidationBehavior<TestNoKeysCommand, Result<bool>>(_cacheService);

        await sut.Handle(new TestNoKeysCommand(), next, CancellationToken.None);

        await _cacheService.DidNotReceive().RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cacheService.DidNotReceive().RemoveByPrefixAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
