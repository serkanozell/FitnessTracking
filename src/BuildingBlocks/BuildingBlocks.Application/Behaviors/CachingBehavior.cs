using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Application.Behaviors
{
    public sealed class CachingBehavior<TRequest, TResponse>(ICacheAsideService cacheService, IOptions<CacheOptions> options)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var defaultExpiration = TimeSpan.FromMinutes(options.Value.DefaultExpirationMinutes);
            var expiration = request.Expiration ?? defaultExpiration;

            return await cacheService.GetOrAddAsync(request.CacheKey,
                                                    async _ => await next(ct),
                                                    expiration,
                                                    ShouldCache,
                                                    ct);
        }

        // Only successful results should be cached; failures (e.g. not found,
        // transient errors) must not be persisted for the whole expiration window.
        private static bool ShouldCache(TResponse response)
            => response is not Result result || result.IsSuccess;
    }
}