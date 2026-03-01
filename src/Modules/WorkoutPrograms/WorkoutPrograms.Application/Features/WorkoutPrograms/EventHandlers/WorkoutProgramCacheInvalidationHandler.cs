using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutPrograms.Domain.Events;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.EventHandlers
{
    internal sealed class WorkoutProgramCacheInvalidationHandler(ICacheService cacheService)
        : IDomainEventHandler<WorkoutProgramCreatedEvent>,
          IDomainEventHandler<WorkoutProgramUpdatedEvent>,
          IDomainEventHandler<WorkoutProgramDeletedEvent>,
          IDomainEventHandler<WorkoutProgramActivatedEvent>,
          IDomainEventHandler<WorkoutProgramSplitChangedEvent>,
          IDomainEventHandler<SplitExerciseChangedEvent>
    {
        public Task Handle(WorkoutProgramCreatedEvent notification, CancellationToken cancellationToken)
            => InvalidateProgramAsync(notification.ProgramId, cancellationToken);

        public Task Handle(WorkoutProgramUpdatedEvent notification, CancellationToken cancellationToken)
            => InvalidateProgramAsync(notification.ProgramId, cancellationToken);

        public Task Handle(WorkoutProgramDeletedEvent notification, CancellationToken cancellationToken)
            => InvalidateProgramAsync(notification.ProgramId, cancellationToken);

        public Task Handle(WorkoutProgramActivatedEvent notification, CancellationToken cancellationToken)
            => InvalidateProgramAsync(notification.ProgramId, cancellationToken);

        public async Task Handle(WorkoutProgramSplitChangedEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"workoutprograms:{notification.ProgramId}", cancellationToken);
            await cacheService.RemoveAsync($"workoutprograms:{notification.ProgramId}:splits", cancellationToken);
        }

        public async Task Handle(SplitExerciseChangedEvent notification, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"workoutprograms:{notification.ProgramId}", cancellationToken);
            await cacheService.RemoveAsync($"workoutprograms:{notification.ProgramId}:splits:{notification.SplitId}:exercises", cancellationToken);
        }

        private async Task InvalidateProgramAsync(Guid programId, CancellationToken cancellationToken)
        {
            await cacheService.RemoveAsync($"workoutprograms:{programId}", cancellationToken);
            await cacheService.RemoveByPrefixAsync("workoutprograms:all", cancellationToken);
        }
    }
}
