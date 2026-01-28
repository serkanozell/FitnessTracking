using FitnessTracking.Application.Dtos;
using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetWorkoutProgramSplitsQueryHandler
    : IRequestHandler<GetWorkoutProgramSplitsQuery, IReadOnlyList<WorkoutProgramSplitDto>>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;

    public GetWorkoutProgramSplitsQueryHandler(IWorkoutProgramRepository workoutProgramRepository)
    {
        _workoutProgramRepository = workoutProgramRepository;
    }

    public async Task<IReadOnlyList<WorkoutProgramSplitDto>> Handle(
        GetWorkoutProgramSplitsQuery request,
        CancellationToken cancellationToken)
    {
        var program = await _workoutProgramRepository
            .GetByIdAsync(request.WorkoutProgramId, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"WorkoutProgram ({request.WorkoutProgramId}) not found.");

        return program.Splits
                      .OrderBy(x => x.Order)
                      .Select(x => new WorkoutProgramSplitDto
                      {
                          Id = x.Id,
                          WorkoutProgramId = x.WorkoutProgramId,
                          Name = x.Name,
                          Order = x.Order
                      })
                      .ToList();
    }
}