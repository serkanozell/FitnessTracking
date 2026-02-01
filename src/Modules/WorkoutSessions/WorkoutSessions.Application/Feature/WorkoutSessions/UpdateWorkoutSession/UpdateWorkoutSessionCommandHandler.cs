using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.UpdateWorkoutSession
{
    internal sealed class UpdateWorkoutSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<UpdateWorkoutSessionCommand, Unit>
    {
        public async Task<Unit> Handle(
            UpdateWorkoutSessionCommand request,
            CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutSession ({request.Id}) not found.");

            session.UpdateDate(request.Date);

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}