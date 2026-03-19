using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Results;
using MediatR;

namespace BuildingBlocks.Application.Behaviors
{
    public sealed class CacheInvalidationBehavior<TRequest, TResponse>(ICacheService cacheService)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheInvalidatingCommand
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var response = await next(ct);

            if (response is Result { IsSuccess: true })
            {
                foreach (var key in request.CacheKeysToInvalidate)
                    await cacheService.RemoveAsync(key, ct);

                foreach (var prefix in request.CachePrefixesToInvalidate)
                    await cacheService.RemoveByPrefixAsync(prefix, ct);
            }

            return response;
        }
    }
}
