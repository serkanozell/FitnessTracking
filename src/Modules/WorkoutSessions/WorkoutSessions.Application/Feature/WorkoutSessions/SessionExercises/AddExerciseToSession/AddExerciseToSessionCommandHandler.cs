using WorkoutPrograms.Application.Services;
using WorkoutSessions.Domain.Repositories;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    internal sealed class AddExerciseToSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository,
                                                             IWorkoutProgramReadService _workoutProgramReadService,
                                                             IWorkoutSessionsUnitOfWork _unitOfWork) : ICommandHandler<AddExerciseToSessionCommand, Guid>
    {
        public async Task<Guid> Handle(
            AddExerciseToSessionCommand request,
            CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(
                request.WorkoutSessionId,
                cancellationToken)
                ?? throw new KeyNotFoundException(
                    $"WorkoutSession ({request.WorkoutSessionId}) not found.");

            var program = await _workoutProgramReadService.GetWorkoutProgramByIdAsync(session.WorkoutProgramId, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutProgram ({session.WorkoutProgramId}) not found.");

            if (!program.ContainsExercise(request.ExerciseId))
            {
                throw new InvalidOperationException($"Exercise ({request.ExerciseId}) is not part of workout program {program.Id}.");
            }

            // Programda bu exercise için tanımlı max set sayısı
            var programExercise = program.Splits.SelectMany(s => s.Exercises).FirstOrDefault(x => x.ExerciseId == request.ExerciseId);

            // Session'da şu anki set sayısı
            var currentSetCountInSession = session.SessionExercises.Count(x => x.ExerciseId == request.ExerciseId);

            if (currentSetCountInSession >= programExercise!.Sets)
            {
                throw new InvalidOperationException($"Exercise ({request.ExerciseId}) for program {program.Id} is limited to {programExercise.Sets} sets. " + $"Session {session.Id} already has {currentSetCountInSession} sets.");
            }

            if (request.SetNumber >= programExercise.Sets)
            {
                throw new InvalidOperationException($"Exercise ({request.ExerciseId}) for program {program.Id} is limited to {programExercise.Sets} sets.");
            }

            var workoutExercise = session.AddEntry(request.ExerciseId,
                                                   request.SetNumber,
                                                   request.Weight,
                                                   request.Reps);

            await _workoutSessionRepository.UpdateAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return workoutExercise.Id;
        }
    }
}