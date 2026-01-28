using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class UpdateWorkoutProgramSplitCommandHandler
    : IRequestHandler<UpdateWorkoutProgramSplitCommand, bool>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;

    public UpdateWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository workoutProgramRepository)
    {
        _workoutProgramRepository = workoutProgramRepository;
    }

    public async Task<bool> Handle(
        UpdateWorkoutProgramSplitCommand request,
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

        program.UpdateSplit(request.SplitId, request.Name, request.Order);

        await _workoutProgramRepository.UpdateAsync(program, cancellationToken);

        return true;
    }
}