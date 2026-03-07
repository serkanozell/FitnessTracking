using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Results;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace BuildingBlocks.Application.UnitTests.Behaviors;

public class CachingBehaviorTests
{
    private sealed record TestCacheableQuery : IQuery<Result<string>>, ICacheableQuery
    {
        public string CacheKey => "test:cache:key";
        public TimeSpan? Expiration { get; init; }
    }

    private readonly ICacheAsideService _cacheService = Substitute.For<ICacheAsideService>();
    private readonly IOptions<CacheOptions> _options = Options.Create(new CacheOptions { DefaultExpirationMinutes = 5 });
    private readonly RequestHandlerDelegate<Result<string>> _next = Substitute.For<RequestHandlerDelegate<Result<string>>>();

    [Fact]
    public async Task Handle_ShouldCallCacheServiceWithCorrectKey()
    {
        var expectedResult = Result<string>.Success("cached");
        _cacheService.GetOrAddAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, Task<Result<string>>>>(),
                Arg.Any<TimeSpan?>(),
                Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        var sut = new CachingBehavior<TestCacheableQuery, Result<string>>(_cacheService, _options);
        var query = new TestCacheableQuery();

        var result = await sut.Handle(query, _next, CancellationToken.None);

        result.Should().Be(expectedResult);
        await _cacheService.Received(1).GetOrAddAsync(
            "test:cache:key",
            Arg.Any<Func<CancellationToken, Task<Result<string>>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultExpiration_WhenQueryExpirationIsNull()
    {
        _cacheService.GetOrAddAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, Task<Result<string>>>>(),
                Arg.Any<TimeSpan?>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<string>.Success("data"));

        var sut = new CachingBehavior<TestCacheableQuery, Result<string>>(_cacheService, _options);
        var query = new TestCacheableQuery { Expiration = null };

        await sut.Handle(query, _next, CancellationToken.None);

        await _cacheService.Received(1).GetOrAddAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<Result<string>>>>(),
            TimeSpan.FromMinutes(5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUseQueryExpiration_WhenProvided()
    {
        var customExpiration = TimeSpan.FromMinutes(30);
        _cacheService.GetOrAddAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, Task<Result<string>>>>(),
                Arg.Any<TimeSpan?>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<string>.Success("data"));

        var sut = new CachingBehavior<TestCacheableQuery, Result<string>>(_cacheService, _options);
        var query = new TestCacheableQuery { Expiration = customExpiration };

        await sut.Handle(query, _next, CancellationToken.None);

        await _cacheService.Received(1).GetOrAddAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<Result<string>>>>(),
            customExpiration,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedResult_WhenCacheHit()
    {
        var cachedResult = Result<string>.Success("from-cache");
        _cacheService.GetOrAddAsync(
                Arg.Any<string>(),
                Arg.Any<Func<CancellationToken, Task<Result<string>>>>(),
                Arg.Any<TimeSpan?>(),
                Arg.Any<CancellationToken>())
            .Returns(cachedResult);

        var sut = new CachingBehavior<TestCacheableQuery, Result<string>>(_cacheService, _options);

        var result = await sut.Handle(new TestCacheableQuery(), _next, CancellationToken.None);

        result.Data.Should().Be("from-cache");
    }
}
