using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class UpdateWorkoutProgramExerciseCommandHandler
    : IRequestHandler<UpdateWorkoutProgramExerciseCommand, bool>
{
    private readonly IWorkoutProgramRepository _programRepository;

    public UpdateWorkoutProgramExerciseCommandHandler(IWorkoutProgramRepository programRepository)
    {
        _programRepository = programRepository;
    }

    public async Task<bool> Handle(
        UpdateWorkoutProgramExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);
        if (program is null)
        {
            return false;
        }

        // Aggregate içinden entity güncelle
        program.UpdateExercise(
            request.WorkoutProgramExerciseId,
            request.Sets,
            request.TargetReps);

        await _programRepository.UpdateAsync(program, cancellationToken);

        return true;
    }
}