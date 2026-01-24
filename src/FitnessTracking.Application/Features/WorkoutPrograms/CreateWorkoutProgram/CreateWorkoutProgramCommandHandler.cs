using FitnessTracking.Domain.Entity;
using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class CreateWorkoutProgramCommandHandler
    : IRequestHandler<CreateWorkoutProgramCommand, Guid>
{
    private readonly IWorkoutProgramRepository _repository;

    public CreateWorkoutProgramCommandHandler(IWorkoutProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateWorkoutProgramCommand request,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        var program = new WorkoutProgram(
            id,
            request.Name,
            request.StartDate,
            request.EndDate);

        await _repository.AddAsync(program, cancellationToken);

        return id;
    }
}