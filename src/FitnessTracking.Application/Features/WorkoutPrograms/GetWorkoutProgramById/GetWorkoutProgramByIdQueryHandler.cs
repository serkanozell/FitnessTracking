using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetWorkoutProgramByIdQueryHandler
    : IRequestHandler<GetWorkoutProgramByIdQuery, WorkoutProgramDto?>
{
    private readonly IWorkoutProgramRepository _repository;

    public GetWorkoutProgramByIdQueryHandler(IWorkoutProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<WorkoutProgramDto?> Handle(
        GetWorkoutProgramByIdQuery request,
        CancellationToken cancellationToken)
    {
        var program = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (program is null)
        {
            return null;
        }

        return WorkoutProgramDto.FromEntity(program);
    }
}