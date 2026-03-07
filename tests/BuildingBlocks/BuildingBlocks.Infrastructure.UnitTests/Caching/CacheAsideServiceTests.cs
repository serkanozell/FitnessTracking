using BuildingBlocks.Application.Abstractions.Caching;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BuildingBlocks.Infrastructure.UnitTests.Caching;

public class CacheAsideServiceTests
{
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly CacheAsideService _sut;

    public CacheAsideServiceTests()
    {
        _sut = new CacheAsideService(_cacheService);
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldReturnCachedValue_WhenCacheHit()
    {
        _cacheService.GetAsync<string>("key", Arg.Any<CancellationToken>()).Returns("cached-value");
        var factoryCalled = false;

        var result = await _sut.GetOrAddAsync("key", _ =>
        {
            factoryCalled = true;
            return Task.FromResult("new-value");
        });

        result.Should().Be("cached-value");
        factoryCalled.Should().BeFalse();
        await _cacheService.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldCallFactoryAndSetCache_WhenCacheMiss()
    {
        _cacheService.GetAsync<string>("key", Arg.Any<CancellationToken>()).Returns((string?)null);

        var result = await _sut.GetOrAddAsync("key", _ => Task.FromResult("factory-value"));

        result.Should().Be("factory-value");
        await _cacheService.Received(1).SetAsync("key", "factory-value", Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldPassExpirationToSet()
    {
        var expiration = TimeSpan.FromMinutes(15);
        _cacheService.GetAsync<string>("counter", Arg.Any<CancellationToken>()).Returns((string?)null);

        await _sut.GetOrAddAsync("counter", _ => Task.FromResult("42"), expiration);

        await _cacheService.Received(1).SetAsync("counter", "42", expiration, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldPassNullExpiration_WhenNotProvided()
    {
        _cacheService.GetAsync<string>("key", Arg.Any<CancellationToken>()).Returns((string?)null);

        await _sut.GetOrAddAsync<string>("key", _ => Task.FromResult("val"));

        await _cacheService.Received(1).SetAsync("key", "val", null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldPassCancellationToken()
    {
        var cts = new CancellationTokenSource();
        _cacheService.GetAsync<string>("key", cts.Token).Returns((string?)null);

        await _sut.GetOrAddAsync("key", _ => Task.FromResult("val"), null, cts.Token);

        await _cacheService.Received(1).GetAsync<string>("key", cts.Token);
        await _cacheService.Received(1).SetAsync("key", "val", null, cts.Token);
    }
}
