using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class AddWorkoutProgramSplitCommandHandler : IRequestHandler<AddWorkoutProgramSplitCommand, Guid>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;

    public AddWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository workoutProgramRepository)
    {
        _workoutProgramRepository = workoutProgramRepository;
    }

    public async Task<Guid> Handle(
        AddWorkoutProgramSplitCommand request,
        CancellationToken cancellationToken)
    {
        // Aggregate root'u, split ve exercises ile birlikte yükle
        var program = await _workoutProgramRepository
            .GetByIdAsync(request.WorkoutProgramId, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"WorkoutProgram ({request.WorkoutProgramId}) not found.");

        // Davranışı aggregate üzerinden yap
        var split = program.AddSplit(request.Name, request.Order);

        await _workoutProgramRepository.UpdateAsync(program, cancellationToken);

        return split.Id;
    }
}