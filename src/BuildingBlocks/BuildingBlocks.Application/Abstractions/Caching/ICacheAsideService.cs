namespace BuildingBlocks.Application.Abstractions.Caching
{
    public interface ICacheAsideService
    {
        Task<T> GetOrAddAsync<T>(string key,
                                 Func<CancellationToken, Task<T>> factory,
                                 TimeSpan? expiration = null,
                                 CancellationToken cancellationToken = default);
    }
}