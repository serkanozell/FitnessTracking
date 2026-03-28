using BuildingBlocks.Application.Abstractions;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession
{
    internal sealed class CreateWorkoutSessionCommandHandler(
        IWorkoutSessionRepository _workoutSessionRepository,
        IWorkoutProgramModule _workoutProgramModule,
        IWorkoutSessionsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<CreateWorkoutSessionCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateWorkoutSessionCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            if (!await _workoutProgramModule.ExistsAsync(request.WorkoutProgramId, cancellationToken))
                return WorkoutSessionErrors.ProgramNotFound(request.WorkoutProgramId);

            if (!_currentUser.IsAdmin && !await _workoutProgramModule.IsOwnedByUserAsync(request.WorkoutProgramId, userId, cancellationToken))
                return Error.Forbidden();

            var session = WorkoutSession.Create(userId, request.WorkoutProgramId, request.Date);

            await _workoutSessionRepository.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}