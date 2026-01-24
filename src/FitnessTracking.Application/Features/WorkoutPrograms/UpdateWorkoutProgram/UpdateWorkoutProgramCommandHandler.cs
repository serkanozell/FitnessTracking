using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class UpdateWorkoutProgramCommandHandler : IRequestHandler<UpdateWorkoutProgramCommand, Unit>
{
    private readonly IWorkoutProgramRepository _repository;

    public UpdateWorkoutProgramCommandHandler(IWorkoutProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateWorkoutProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (program is null)
        {
            // Burada kendi NotFound exception tipini kullanabilirsin
            throw new KeyNotFoundException($"WorkoutProgram ({request.Id}) not found.");
        }

        program.Update(request.Name, request.StartDate, request.EndDate);

        await _repository.UpdateAsync(program, cancellationToken);

        return Unit.Value;
    }
}