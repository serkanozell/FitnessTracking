using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxBackgroundService : BackgroundService
    {
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
                    await ProcessOutboxMessagesAsync(stoppingToken);
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

        private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OutboxDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var messages = await dbContext.OutboxMessages.Where(m => !m.IsProcessed && m.ProcessedOnUtc == null)
                                                         .OrderBy(m => m.OccurredOnUtc)
                                                         .Take(_options.BatchSize)
                                                         .ToListAsync(cancellationToken);

            if (messages.Count == 0)
                return;

            _logger.LogInformation("Processing {Count} outbox messages", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    var eventType = Type.GetType(message.EventType);
                    if (eventType is null)
                    {
                        _logger.LogWarning("Could not resolve event type: {EventType}", message.EventType);
                        message.IsProcessed = true;
                        message.ProcessedOnUtc = DateTime.UtcNow;
                        message.Error = $"Could not resolve type: {message.EventType}";
                        continue;
                    }

                    var domainEvent = System.Text.Json.JsonSerializer.Deserialize(message.Content, eventType);
                    if (domainEvent is null)
                    {
                        _logger.LogWarning("Could not deserialize outbox message {MessageId}", message.Id);
                        message.IsProcessed = true;
                        message.ProcessedOnUtc = DateTime.UtcNow;
                        message.Error = "Deserialization returned null";
                        continue;
                    }

                    await mediator.Publish(domainEvent, cancellationToken);

                    message.IsProcessed = true;
                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = null;

                    _logger.LogDebug("Successfully processed outbox message {MessageId} of type {EventType}",
                        message.Id, message.EventType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                    message.Error = ex.ToString();
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
