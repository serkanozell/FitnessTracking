using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetWorkoutProgramListQueryHandler
    : IRequestHandler<GetWorkoutProgramListQuery, IReadOnlyList<WorkoutProgramDto>>
{
    private readonly IWorkoutProgramRepository _repository;

    public GetWorkoutProgramListQueryHandler(IWorkoutProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<WorkoutProgramDto>> Handle(
        GetWorkoutProgramListQuery request,
        CancellationToken cancellationToken)
    {
        var programs = await _repository.GetListAsync(cancellationToken);

        return programs
            .Select(WorkoutProgramDto.FromEntity)
            .ToArray();
    }
}