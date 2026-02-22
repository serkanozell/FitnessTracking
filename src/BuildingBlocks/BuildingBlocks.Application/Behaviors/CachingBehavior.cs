using BuildingBlocks.Application.Abstractions.Caching;
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
                                                    ct);
        }
    }
}