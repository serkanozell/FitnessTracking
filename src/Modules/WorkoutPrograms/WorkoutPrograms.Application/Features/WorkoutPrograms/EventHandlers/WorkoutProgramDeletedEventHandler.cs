using WorkoutPrograms.Contracts.Events;
using WorkoutPrograms.Domain.Events;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.EventHandlers
{
    internal sealed class WorkoutProgramDeletedEventHandler(IPublisher _publisher) : IDomainEventHandler<WorkoutProgramDeletedEvent>
    {
        public async Task Handle(WorkoutProgramDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new WorkoutProgramDeletedIntegrationEvent(notification.ProgramId), cancellationToken);
        }
    }
}
