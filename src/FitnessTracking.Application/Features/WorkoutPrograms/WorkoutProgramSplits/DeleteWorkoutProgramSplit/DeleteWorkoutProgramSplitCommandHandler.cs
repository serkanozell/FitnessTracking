using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class DeleteWorkoutProgramSplitCommandHandler
    : IRequestHandler<DeleteWorkoutProgramSplitCommand, bool>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;

    public DeleteWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository workoutProgramRepository)
    {
        _workoutProgramRepository = workoutProgramRepository;
    }

    public async Task<bool> Handle(
        DeleteWorkoutProgramSplitCommand request,
        CancellationToken cancellationToken)
    {
        var program = await _workoutProgramRepository
            .GetByIdAsync(request.WorkoutProgramId, cancellationToken);

        if (program is null)
        {
            return false;
        }

        var split = program.Splits
            .SingleOrDefault(x => x.Id == request.SplitId);

        if (split is null)
        {
            return false;
        }

        program.RemoveSplit(request.SplitId);

        await _workoutProgramRepository.UpdateAsync(program, cancellationToken);

        return true;
    }
}