using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Application.Abstractions.Idempotency;
using BuildingBlocks.Application.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Application.Behaviors
{
    // Guards command handlers against duplicate execution. When a command carries
    // an idempotency key and a successful response for that key is already cached,
    // the cached response is replayed instead of re-running the handler. Only
    // successful Results are stored, so a failed attempt can be retried.
    public sealed class IdempotencyBehavior<TRequest, TResponse>(
        ICacheService cacheService,
        IOptions<IdempotencyOptions> options)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IIdempotentCommand
    {
        private const string KeyPrefix = "idempotency:";

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
                return await next(ct);

            var cacheKey = $"{KeyPrefix}{request.IdempotencyKey}";

            var cached = await cacheService.GetAsync<TResponse>(cacheKey, ct);
            if (cached is not null)
                return cached;

            var response = await next(ct);

            if (response is not Result result || result.IsSuccess)
            {
                var expiration = TimeSpan.FromMinutes(options.Value.ExpirationMinutes);
                await cacheService.SetAsync(cacheKey, response, expiration, ct);
            }

            return response;
        }
    }
}
