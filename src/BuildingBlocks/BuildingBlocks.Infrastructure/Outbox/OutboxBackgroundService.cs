using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxBackgroundService : BackgroundService
    {
        /// <summary>
        /// <see cref="Type.GetType(string)"/> reflection sonuçlarını process ömrü boyunca cache'ler.
        /// Aynı <c>EventType</c> string'i her zaman aynı CLR tipine çözüldüğünden, her mesajda
        /// reflection maliyetini tekrar ödemeyiz. Çözülemeyen (null) tipler cache'lenmez ki
        /// ilgili assembly daha sonra yüklenirse yeniden denenebilsin.
        /// </summary>
        private static readonly ConcurrentDictionary<string, Type> _eventTypeCache = new(StringComparer.Ordinal);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly OutboxOptions _options;
        private readonly ILogger<OutboxBackgroundService> _logger;

        public OutboxBackgroundService(IServiceScopeFactory scopeFactory,
                                       IOptions<OutboxOptions> options,
                                       ILogger<OutboxBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox background service started. Interval: {Interval}s, BatchSize: {BatchSize}",
                _options.IntervalInSeconds, _options.BatchSize);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DrainOutboxAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing outbox messages");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalInSeconds), stoppingToken);
            }
        }

        /// <summary>
        /// Bir tetikleme döngüsünde, batch tamamen dolduğu (yani işlenmeyi bekleyen daha
        /// fazla mesaj olduğu) sürece ardışık batch'leri <c>Task.Delay</c> beklemeden işler.
        /// Böylece birikmiş mesajlar interval'i beklemeden boşaltılır (drain). Tek bir
        /// tetiklemede DB'yi sürekli sorgulamayı önlemek için <see cref="OutboxOptions.MaxDrainIterations"/>
        /// güvenlik tavanı uygulanır; tavana ulaşılırsa kalan mesajlar bir sonraki interval'de işlenir.
        /// </summary>
        internal async Task DrainOutboxAsync(CancellationToken cancellationToken)
        {
            for (var iteration = 0; iteration < _options.MaxDrainIterations; iteration++)
            {
                var processedCount = await ProcessOutboxMessagesAsync(cancellationToken);

                // Batch tam dolu değilse bekleyen iş kalmamıştır; interval delay'ine geri dön.
                if (processedCount < _options.BatchSize)
                    return;

                _logger.LogDebug("Outbox batch full ({BatchSize}); draining next batch immediately (iteration {Iteration}).",
                    _options.BatchSize, iteration + 1);
            }

            _logger.LogInformation("Outbox drain reached MaxDrainIterations ({MaxDrainIterations}); remaining messages will be processed on next interval.",
                _options.MaxDrainIterations);
        }

        internal async Task<int> ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var messages = await dbContext.OutboxMessages
                                         .Where(m => !m.IsProcessed && m.RetryCount < _options.MaxRetries)
                                         .OrderBy(m => m.OccurredOnUtc)
                                         .Take(_options.BatchSize)
                                         .ToListAsync(cancellationToken);

            if (messages.Count == 0)
                return 0;

            _logger.LogInformation("Processing {Count} outbox messages", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    var eventType = ResolveEventType(message.EventType);
                    if (eventType is null)
                    {
                        _logger.LogWarning("Could not resolve event type: {EventType}. Moving to dead letter.", message.EventType);
                        MarkAsDeadLetter(message, $"Could not resolve type: {message.EventType}");
                        continue;
                    }

                    var domainEvent = System.Text.Json.JsonSerializer.Deserialize(message.Content, eventType);
                    if (domainEvent is null)
                    {
                        _logger.LogWarning("Could not deserialize outbox message {MessageId}. Moving to dead letter.", message.Id);
                        MarkAsDeadLetter(message, "Deserialization returned null");
                        continue;
                    }

                    await mediator.Publish(domainEvent, cancellationToken);

                    message.IsProcessed = true;
                    message.ProcessedOnUtc = DateTime.Now;
                    message.Error = null;

                    _logger.LogDebug("Successfully processed outbox message {MessageId} of type {EventType}",
                        message.Id, message.EventType);
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.ToString();

                    if (message.RetryCount >= _options.MaxRetries)
                    {
                        MarkAsDeadLetter(message, ex.ToString());
                        _logger.LogError(ex, "Outbox message {MessageId} exceeded max retries ({MaxRetries}). Moved to dead letter.",
                            message.Id, _options.MaxRetries);
                    }
                    else
                    {
                        _logger.LogWarning(ex, "Failed to process outbox message {MessageId}. Retry {RetryCount}/{MaxRetries}.",
                            message.Id, message.RetryCount, _options.MaxRetries);
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return messages.Count;
        }

        /// <summary>
        /// <c>EventType</c> string'ini CLR tipine çözer ve sonucu <see cref="_eventTypeCache"/>
        /// içinde önbelleğe alır. Çözülemeyen tipler için <c>null</c> döner ve cache'lenmez,
        /// böylece ilgili assembly sonradan yüklenirse yeniden denenir.
        /// </summary>
        private static Type? ResolveEventType(string eventTypeName)
        {
            if (_eventTypeCache.TryGetValue(eventTypeName, out var cached))
                return cached;

            var resolved = Type.GetType(eventTypeName);
            if (resolved is not null)
                _eventTypeCache.TryAdd(eventTypeName, resolved);

            return resolved;
        }

        private static void MarkAsDeadLetter(OutboxMessage message, string error)
        {
            message.IsProcessed = true;
            message.ProcessedOnUtc = DateTime.Now;
            message.Error = error;
        }
    }
}