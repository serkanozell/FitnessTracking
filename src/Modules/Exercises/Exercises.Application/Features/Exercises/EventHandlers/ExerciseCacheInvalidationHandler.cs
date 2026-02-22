using BuildingBlocks.Application.Abstractions.Caching;
using Exercises.Domain.Events;

namespace Exercises.Application.Features.Exercises.EventHandlers
{
    internal sealed class ExerciseCacheInvalidationHandler(ICacheService cacheService)
        : IDomainEventHandler<ExerciseCreatedEvent>,
          IDomainEventHandler<ExerciseUpdatedEvent>,
          IDomainEventHandler<ExerciseDeletedEvent>,
          IDomainEventHandler<ExerciseActivatedEvent>
    {
        public Task Handle(ExerciseCreatedEvent notification, CancellationToken cancellationToken)
            => InvalidateAsync(notification.ExerciseId, cancellationToken);

        public Task Handle(ExerciseUpdatedEvent notification, CancellationToken cancellationToken)
            => InvalidateAsync(notification.ExerciseId, cancellationToken);

        public Task Handle(ExerciseDeletedEvent notification, CancellationToken cancellationToken)
            => InvalidateAsync(notification.ExerciseId, cancellationToken);

        public Task Handle(ExerciseActivatedEvent notification, CancellationToken cancellationToken)
            => InvalidateAsync(notification.ExerciseId, cancellationToken);

        private async Task InvalidateAsync(Guid exerciseId, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"exercises:{exerciseId}", cancellationToken);
            await cacheService.RemoveAsync("exercises:all", cancellationToken);
        }
    }
}
