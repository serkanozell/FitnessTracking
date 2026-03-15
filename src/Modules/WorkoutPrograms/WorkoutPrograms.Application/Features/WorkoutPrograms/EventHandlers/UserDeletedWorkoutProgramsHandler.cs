using Users.Contracts.Events;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.EventHandlers
{
    internal sealed class UserDeletedWorkoutProgramsHandler(
        IWorkoutProgramRepository _repository,
        IWorkoutProgramsUnitOfWork _unitOfWork) : IDomainEventHandler<UserDeletedIntegrationEvent>
    {
        public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var programs = await _repository.GetActiveByUserIdAsync(notification.UserId, cancellationToken);

            foreach (var program in programs)
            {
                program.Delete(notification.PerformedBy);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}