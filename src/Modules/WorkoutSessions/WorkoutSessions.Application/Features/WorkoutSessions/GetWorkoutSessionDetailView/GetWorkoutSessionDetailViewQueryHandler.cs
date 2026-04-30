using BuildingBlocks.Application.Abstractions;
using Exercises.Contracts;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Application.Dtos;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionDetailView;

internal sealed class GetWorkoutSessionDetailViewQueryHandler(
    IWorkoutSessionRepository _workoutSessionRepository,
    IWorkoutProgramModule _workoutProgramModule,
    IExerciseModule _exerciseModule,
    ICurrentUser _currentUser)
    : IQueryHandler<GetWorkoutSessionDetailViewQuery, Result<WorkoutSessionDetailViewDto>>
{
    public async Task<Result<WorkoutSessionDetailViewDto>> Handle(
        GetWorkoutSessionDetailViewQuery request, CancellationToken cancellationToken)
    {
        var session = await _workoutSessionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (session is null)
            return WorkoutSessionErrors.NotFound(request.Id);

        var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, session.UserId);
        if (ownershipError is not null)
            return ownershipError;

        // Fan out cross-module lookups in parallel within the API process so the MVC tier
        // doesn't have to make multiple sequential HTTP calls.
        var programTask = _workoutProgramModule.GetProgramWithSplitsAsync(session.WorkoutProgramId, cancellationToken);
        var exercisesTask = _exerciseModule.GetExercisesAsync(cancellationToken);

        await Task.WhenAll(programTask, exercisesTask);

        var program = programTask.Result;
        var allExercises = exercisesTask.Result;

        var exerciseNameById = allExercises.ToDictionary(e => e.Id, e => e.Name);

        var programSplits = program?.Splits
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Order)
            .Select(s => new SessionSplitInfoDto(s.Id, s.Name, s.Order))
            .ToList()
            ?? [];

        var sessionSplitExercises = program?.Splits
            .FirstOrDefault(s => s.Id == session.WorkoutProgramSplitId)?
            .Exercises
            .Where(x => !x.IsDeleted)
            .Select(x => new SessionSplitExerciseInfoDto(
                x.ExerciseId,
                exerciseNameById.GetValueOrDefault(x.ExerciseId, string.Empty)))
            .OrderBy(x => x.Name)
            .ToList()
            ?? [];

        // Only the exercise names actually referenced by the session are needed by the view.
        var referencedNames = session.SessionExercises
            .Select(e => e.ExerciseId)
            .Distinct()
            .ToDictionary(id => id, id => exerciseNameById.GetValueOrDefault(id, string.Empty));

        return new WorkoutSessionDetailViewDto
        {
            Session = WorkoutSessionDetailDto.FromEntity(session),
            ProgramName = program?.Name ?? string.Empty,
            ProgramSplits = programSplits,
            SessionSplitExercises = sessionSplitExercises,
            ExerciseNames = referencedNames
        };
    }
}
