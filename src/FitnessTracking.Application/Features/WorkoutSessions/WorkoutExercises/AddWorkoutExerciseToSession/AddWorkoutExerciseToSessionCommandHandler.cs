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

        // Programda bu exercise için tanımlı max set sayısı
        var programExercise = program.WorkoutProgramExercises.FirstOrDefault(x => x.ExerciseId == request.ExerciseId);

        // Session'da şu anki set sayısı
        var currentSetCountInSession = session.WorkoutExercises.Count(x => x.ExerciseId == request.ExerciseId);

        if (currentSetCountInSession >= programExercise!.Sets)
        {
            throw new InvalidOperationException($"Exercise ({request.ExerciseId}) for program {program.Id} is limited to {programExercise.Sets} sets. " + $"Session {session.Id} already has {currentSetCountInSession} sets.");
        }

        var workoutExercise = session.AddEntry(request.ExerciseId,
                                               request.SetNumber,
                                               request.Weight,
                                               request.Reps);

        await _sessionRepository.UpdateAsync(session, cancellationToken);

        return workoutExercise.Id;
    }
}