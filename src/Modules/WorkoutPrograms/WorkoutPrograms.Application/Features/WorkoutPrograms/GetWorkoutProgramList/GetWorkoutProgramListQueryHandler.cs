using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using Exercises.Contracts;
using WorkoutPrograms.Application.Dtos;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    internal sealed class GetWorkoutProgramListQueryHandler(
        IWorkoutProgramRepository _workoutProgramRepository,
        IExerciseModule _exerciseModule,
        ICurrentUser _currentUser) : IQueryHandler<GetWorkoutProgramListQuery, Result<PagedResult<WorkoutProgramDto>>>
    {
        public async Task<Result<PagedResult<WorkoutProgramDto>>> Handle(GetWorkoutProgramListQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var (items, totalCount) = await _workoutProgramRepository.GetPagedByUserAsync(userId, request.PageNumber, request.PageSize, cancellationToken);

            var allExercises = await _exerciseModule.GetExercisesAsync(cancellationToken);

            var dtos = items.Select(p => WorkoutProgramDto.FromEntity(p, allExercises)).ToList();

            return PagedResult<WorkoutProgramDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}