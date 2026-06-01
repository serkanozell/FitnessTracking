namespace BuildingBlocks.Application.Abstractions.Caching
{
    public interface ICacheAsideService
    {
        Task<T> GetOrAddAsync<T>(string key,
                                 Func<CancellationToken, Task<T>> factory,
                                 TimeSpan? expiration = null,
                                 Func<T, bool>? shouldCache = null,
                                 CancellationToken cancellationToken = default);
    }
}