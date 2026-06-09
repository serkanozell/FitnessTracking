using BuildingBlocks.Application.Abstractions.Caching;
using FluentAssertions;
using NSubstitute;
using System.Reflection;
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

        await _sut.GetOrAddAsync("key", _ => Task.FromResult("val"), null, null, cts.Token);

        await _cacheService.Received().GetAsync<string>("key", cts.Token);
        await _cacheService.Received(1).SetAsync("key", "val", null, cts.Token);
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldNotCache_WhenShouldCachePredicateReturnsFalse()
    {
        _cacheService.GetAsync<string>("key", Arg.Any<CancellationToken>()).Returns((string?)null);

        var result = await _sut.GetOrAddAsync("key", _ => Task.FromResult("skip-me"), null, _ => false);

        result.Should().Be("skip-me");
        await _cacheService.DidNotReceive().SetAsync("key", Arg.Any<string>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldCache_WhenShouldCachePredicateReturnsTrue()
    {
        _cacheService.GetAsync<string>("key", Arg.Any<CancellationToken>()).Returns((string?)null);

        var result = await _sut.GetOrAddAsync("key", _ => Task.FromResult("keep-me"), null, _ => true);

        result.Should().Be("keep-me");
        await _cacheService.Received(1).SetAsync("key", "keep-me", Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldCallFactoryExactlyOnce_WhenManyCallersRaceOnSameKey()
    {
        const string key = "stampede-key";

        // Model a real cache: every caller sees a miss until the factory result is stored,
        // after which subsequent (double-check) reads return the cached value.
        string? stored = null;
        var gate = new object();
        _cacheService.GetAsync<string>(key, Arg.Any<CancellationToken>())
            .Returns(_ => { lock (gate) { return stored; } });
        _cacheService.SetAsync(key, Arg.Any<string>(), Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>())
            .Returns(ci => { lock (gate) { stored = ci.ArgAt<string>(1); } return Task.CompletedTask; });

        var factoryCalls = 0;
        var start = new TaskCompletionSource();

        async Task<string> Factory(CancellationToken _)
        {
            Interlocked.Increment(ref factoryCalls);
            await Task.Delay(50); // hold the lock long enough for the herd to pile up
            return "rebuilt-value";
        }

        var callers = Enumerable.Range(0, 20).Select(async _ =>
        {
            await start.Task;
            return await _sut.GetOrAddAsync(key, Factory);
        }).ToArray();

        start.SetResult();
        var results = await Task.WhenAll(callers);

        factoryCalls.Should().Be(1, "only one caller should rebuild the value while the rest await it");
        results.Should().OnlyContain(r => r == "rebuilt-value");
        await _cacheService.Received(1).SetAsync(key, "rebuilt-value", Arg.Any<TimeSpan?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrAddAsync_ShouldNotLeakLocks_AfterCompletion()
    {
        const string key = "leak-check-key";
        _cacheService.GetAsync<string>(key, Arg.Any<CancellationToken>()).Returns((string?)null);

        await _sut.GetOrAddAsync(key, _ => Task.FromResult("value"));

        GetLockCount(key).Should().Be(0, "the per-key lock must be removed once the last waiter releases it");
    }

    // Reads the static reference-counted lock dictionary via reflection to assert the
    // key's SemaphoreSlim was removed (no thundering-herd lock leak for the singleton service).
    private static int GetLockCount(string key)
    {
        var field = typeof(CacheAsideService).GetField("_locks", BindingFlags.NonPublic | BindingFlags.Static)!;
        var dict = field.GetValue(null)!;
        var countProp = dict.GetType().GetProperty("Count")!;
        return (int)countProp.GetValue(dict)!;
    }
}
