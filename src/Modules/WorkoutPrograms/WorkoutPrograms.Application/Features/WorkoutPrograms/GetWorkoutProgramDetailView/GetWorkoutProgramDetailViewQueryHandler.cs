using BuildingBlocks.Application.Abstractions;
using Exercises.Contracts;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramDetailView;

internal sealed class GetWorkoutProgramDetailViewQueryHandler(
    IWorkoutProgramRepository _workoutProgramRepository,
    IExerciseModule _exerciseModule,
    ICurrentUser _currentUser)
    : IQueryHandler<GetWorkoutProgramDetailViewQuery, Result<WorkoutProgramDetailViewDto>>
{
    public async Task<Result<WorkoutProgramDetailViewDto>> Handle(
        GetWorkoutProgramDetailViewQuery request, CancellationToken cancellationToken)
    {
        // Fan out the program load and the exercises lookup in parallel; both are independent.
        var programTask = _workoutProgramRepository.GetByIdWithExercisesAsync(request.Id, cancellationToken);
        var exercisesTask = _exerciseModule.GetExercisesAsync(cancellationToken);

        await Task.WhenAll(programTask, exercisesTask);

        var program = programTask.Result;
        if (program is null)
            return WorkoutProgramErrors.NotFound(request.Id);

        var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, program.UserId);
        if (ownershipError is not null)
            return ownershipError;

        var allExercises = exercisesTask.Result;

        return new WorkoutProgramDetailViewDto
        {
            Program = WorkoutProgramDto.FromEntity(program, allExercises),
            AllExercises = allExercises
                .Where(e => true) // future filter (e.g. active only) can plug here
                .OrderBy(e => e.Name)
                .Select(e => new ExerciseLookupDto(e.Id, e.Name))
                .ToList()
        };
    }
}
