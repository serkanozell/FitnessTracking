using FitnessTracking.Domain.Entity;
using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class CreateExerciseCommandHandler
    : IRequestHandler<CreateExerciseCommand, Guid>
{
    private readonly IExerciseRepository _exerciseRepository;

    public CreateExerciseCommandHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<Guid> Handle(
        CreateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        var exercise = new Exercises.Domain.Entity.Exercise(
            id,
            request.Name,
            request.MuscleGroup,
            request.Description);

        await _exerciseRepository.AddAsync(exercise, cancellationToken);

        return id;
    }
}