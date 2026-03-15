using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Contracts.Events;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.EventHandlers
{
    internal sealed class WorkoutProgramDeletedSessionsHandler(
        IWorkoutSessionRepository _repository,
        IWorkoutSessionsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : IDomainEventHandler<WorkoutProgramDeletedIntegrationEvent>
    {
        public async Task Handle(WorkoutProgramDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var sessions = await _repository.GetActiveByProgramIdAsync(notification.ProgramId, cancellationToken);

            if (sessions.Count == 0) return;

            _currentUser.SetSystemActor(notification.PerformedBy);
            try
            {
                foreach (var session in sessions)
                {
                    session.Delete();
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                _currentUser.ClearSystemActor();
            }
        }
    }
}