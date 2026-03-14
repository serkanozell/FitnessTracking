using BuildingBlocks.Application.Abstractions;
using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionById;
using WorkoutSessions.Domain.Repositories;

internal sealed class GetWorkoutSessionByIdQueryHandler(
    IWorkoutSessionRepository _workoutSessionRepository,
    ICurrentUser _currentUser) : IQueryHandler<GetWorkoutSessionByIdQuery, Result<WorkoutSessionDetailDto>>
{
    public async Task<Result<WorkoutSessionDetailDto>> Handle(GetWorkoutSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return WorkoutSessionErrors.NotFound(request.Id);

        var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, session.UserId);
        if (ownershipError is not null)
            return ownershipError;

        return WorkoutSessionDetailDto.FromEntity(session);
    }
}