using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class RemoveWorkoutProgramExerciseCommandHandler
    : IRequestHandler<RemoveWorkoutProgramExerciseCommand, bool>
{
    private readonly IWorkoutProgramRepository _programRepository;

    public RemoveWorkoutProgramExerciseCommandHandler(IWorkoutProgramRepository programRepository)
    {
        _programRepository = programRepository;
    }

    public async Task<bool> Handle(
        RemoveWorkoutProgramExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);
        if (program is null)
        {
            return false;
        }

        // Aggregate içinden sil
        program.RemoveExercise(request.WorkoutProgramExerciseId);

        await _programRepository.UpdateAsync(program, cancellationToken);

        return true;
    }
}