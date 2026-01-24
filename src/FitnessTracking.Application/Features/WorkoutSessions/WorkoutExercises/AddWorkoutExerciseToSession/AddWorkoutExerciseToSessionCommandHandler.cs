using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class AddWorkoutExerciseToSessionCommandHandler
        : IRequestHandler<AddWorkoutExerciseToSessionCommand, Guid>
{
    private readonly IWorkoutSessionRepository _sessionRepository;
    private readonly IWorkoutProgramRepository _programRepository;

    public AddWorkoutExerciseToSessionCommandHandler(
        IWorkoutSessionRepository sessionRepository,
        IWorkoutProgramRepository programRepository)
    {
        _sessionRepository = sessionRepository;
        _programRepository = programRepository;
    }

    public async Task<Guid> Handle(
        AddWorkoutExerciseToSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(
            request.WorkoutSessionId,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                $"WorkoutSession ({request.WorkoutSessionId}) not found.");

        var program = await _programRepository.GetByIdAsync(
            session.WorkoutProgramId,
            cancellationToken)
            ?? throw new KeyNotFoundException(
                $"WorkoutProgram ({session.WorkoutProgramId}) not found.");

        if (!program.ContainsExercise(request.ExerciseId))
        {
            throw new InvalidOperationException(
                $"Exercise ({request.ExerciseId}) is not part of workout program {program.Id}.");
        }

        var workoutExercise = session.AddEntry(
            request.ExerciseId,
            request.SetNumber,
            request.Weight,
            request.Reps);

        await _sessionRepository.UpdateAsync(session, cancellationToken);

        return workoutExercise.Id;
    }
}