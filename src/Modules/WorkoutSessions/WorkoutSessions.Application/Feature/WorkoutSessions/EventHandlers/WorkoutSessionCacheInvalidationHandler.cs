using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutSessions.Domain.Events;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.EventHandlers
{
    internal sealed class WorkoutSessionCacheInvalidationHandler(ICacheService cacheService)
        : IDomainEventHandler<WorkoutSessionCreatedEvent>,
          IDomainEventHandler<WorkoutSessionUpdatedEvent>,
          IDomainEventHandler<WorkoutSessionDeletedEvent>,
          IDomainEventHandler<WorkoutSessionActivatedEvent>,
          IDomainEventHandler<SessionExerciseChangedEvent>
    {
        public async Task Handle(WorkoutSessionCreatedEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync("workoutsessions:all", cancellationToken);
            await cacheService.RemoveAsync($"workoutsessions:program:{notification.ProgramId}", cancellationToken);
        }

        public async Task Handle(WorkoutSessionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"workoutsessions:{notification.SessionId}", cancellationToken);
            await cacheService.RemoveAsync("workoutsessions:all", cancellationToken);
            await cacheService.RemoveAsync($"workoutsessions:program:{notification.ProgramId}", cancellationToken);
        }

        public async Task Handle(WorkoutSessionDeletedEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"workoutsessions:{notification.SessionId}", cancellationToken);
            await cacheService.RemoveAsync("workoutsessions:all", cancellationToken);
            await cacheService.RemoveAsync($"workoutsessions:program:{notification.ProgramId}", cancellationToken);
        }

        public async Task Handle(WorkoutSessionActivatedEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"workoutsessions:{notification.SessionId}", cancellationToken);
            await cacheService.RemoveAsync("workoutsessions:all", cancellationToken);
        }

        public async Task Handle(SessionExerciseChangedEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"workoutsessions:{notification.SessionId}", cancellationToken);
            await cacheService.RemoveAsync($"workoutsessions:{notification.SessionId}:exercises", cancellationToken);
        }
    }
}
