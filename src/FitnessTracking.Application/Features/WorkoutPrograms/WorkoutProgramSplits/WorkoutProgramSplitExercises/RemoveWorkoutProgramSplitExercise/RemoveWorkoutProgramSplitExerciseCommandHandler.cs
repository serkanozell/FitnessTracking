using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class RemoveWorkoutProgramSplitExerciseCommandHandler
    : IRequestHandler<RemoveWorkoutProgramSplitExerciseCommand, bool>
{
    private readonly IWorkoutProgramRepository _programRepository;

    public RemoveWorkoutProgramSplitExerciseCommandHandler(IWorkoutProgramRepository programRepository)
    {
        _programRepository = programRepository;
    }

    public async Task<bool> Handle(
        RemoveWorkoutProgramSplitExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var program = await _programRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken);
        if (program is null)
        {
            return false;
        }

        var split = program.Splits.SingleOrDefault(x => x.Id == request.WorkoutProgramSplitId);

        if (split is null)
        {
            return false;
        }

        var exercise = split.Exercises.SingleOrDefault(x => x.Id == request.WorkoutProgramExerciseId);

        if (exercise is null)
        {
            return false;
        }

        // Aggregate içinden sil
        split.RemoveExercise(request.WorkoutProgramExerciseId);

        await _programRepository.UpdateAsync(program, cancellationToken);

        return true;
    }
}