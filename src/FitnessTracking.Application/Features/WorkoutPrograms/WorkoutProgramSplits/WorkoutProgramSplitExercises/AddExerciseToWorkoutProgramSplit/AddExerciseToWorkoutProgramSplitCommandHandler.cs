using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class AddExerciseToWorkoutProgramSplitCommandHandler
    : IRequestHandler<AddExerciseToWorkoutProgramSplitCommand, Guid>
{
    private readonly IWorkoutProgramRepository _programRepository;

    public AddExerciseToWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository programRepository)
    {
        _programRepository = programRepository;
    }

    public async Task<Guid> Handle(
        AddExerciseToWorkoutProgramSplitCommand request,
        CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);
        if (program is null)
        {
            throw new KeyNotFoundException(
                $"WorkoutProgram ({request.WorkoutProgramId}) not found.");
        }

        var split = program.Splits.SingleOrDefault(x => x.Id == request.WorkoutProgramSplitId);

        if (split is null)
        {
            throw new KeyNotFoundException(
                $"WorkoutProgramSplit ({request.WorkoutProgramSplitId}) not found in program {request.WorkoutProgramId}.");
        }


        // Entity yönetimi aggregate içinde
        var programExercise = split.AddExercise(request.ExerciseId,
                                                request.Sets,
                                                request.TargetReps);

        await _programRepository.UpdateAsync(program, cancellationToken);

        return programExercise.Id;
    }
}