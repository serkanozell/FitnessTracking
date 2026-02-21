using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.UpdateWorkoutSession
{
    internal sealed class UpdateWorkoutSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<UpdateWorkoutSessionCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (session is null)
                return WorkoutSessionErrors.NotFound(request.Id);

            session.UpdateDate(request.Date);

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}