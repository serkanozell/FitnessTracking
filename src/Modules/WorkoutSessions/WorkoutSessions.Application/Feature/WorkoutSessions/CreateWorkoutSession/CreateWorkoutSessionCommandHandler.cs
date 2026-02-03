using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.CreateWorkoutSession
{
    internal sealed class CreateWorkoutSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<CreateWorkoutSessionCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateWorkoutSessionCommand request,
            CancellationToken cancellationToken)
        {
            var session = WorkoutSession.Create(request.WorkoutProgramId,
                                                           request.Date);

            await _workoutSessionRepository.AddAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}