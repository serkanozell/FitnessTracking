using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class AddExerciseToWorkoutProgramCommandHandler
    : IRequestHandler<AddExerciseToWorkoutProgramCommand, Guid>
{
    private readonly IWorkoutProgramRepository _programRepository;

    public AddExerciseToWorkoutProgramCommandHandler(IWorkoutProgramRepository programRepository)
    {
        _programRepository = programRepository;
    }

    public async Task<Guid> Handle(
        AddExerciseToWorkoutProgramCommand request,
        CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);
        if (program is null)
        {
            throw new KeyNotFoundException(
                $"WorkoutProgram ({request.WorkoutProgramId}) not found.");
        }

        // Entity yönetimi aggregate içinde
        var programExercise = program.AddExercise(
            request.ExerciseId,
            request.Sets,
            request.TargetReps);

        await _programRepository.UpdateAsync(program, cancellationToken);

        return programExercise.Id;
    }
}