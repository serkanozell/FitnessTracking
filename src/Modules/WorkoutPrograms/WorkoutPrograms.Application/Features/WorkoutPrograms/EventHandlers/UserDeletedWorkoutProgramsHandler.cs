using BuildingBlocks.Application.Abstractions;
using Users.Contracts.Events;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.EventHandlers
{
    internal sealed class UserDeletedWorkoutProgramsHandler(
        IWorkoutProgramRepository _repository,
        IWorkoutProgramsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : IDomainEventHandler<UserDeletedIntegrationEvent>
    {
        public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var programs = await _repository.GetActiveByUserIdAsync(notification.UserId, cancellationToken);

            if (programs.Count == 0) return;

            _currentUser.SetSystemActor(notification.PerformedBy);
            try
            {
                foreach (var program in programs)
                {
                    program.Delete(notification.PerformedBy);
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