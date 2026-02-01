using MediatR;
using WorkoutSessions.Domain.Repositories;

namespace FitnessTracking.Application.Features.WorkoutSessions.WorkoutExercises.AddWorkoutExerciseToSession
{
    internal sealed class AddExerciseToSessionCommandHandler(IWorkoutSessionRepository _workoutSessionRepository, IWorkoutSessionsUnitOfWork _unitOfWork) : IRequestHandler<AddWorkoutExerciseToSessionCommand, Guid>
    {
        public async Task<Guid> Handle(
            AddWorkoutExerciseToSessionCommand request,
            CancellationToken cancellationToken)
        {
            var session = await _workoutSessionRepository.GetByIdAsync(
                request.WorkoutSessionId,
                cancellationToken)
                ?? throw new KeyNotFoundException(
                    $"WorkoutSession ({request.WorkoutSessionId}) not found.");

            // workoutprogram modülünden read service açıp burada program var mı diye kontrol edeceğiz
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
            var programExercise = program.Splits.SelectMany(s => s.Exercises).FirstOrDefault(x => x.ExerciseId == request.ExerciseId);

            // Session'da şu anki set sayısı
            var currentSetCountInSession = session.SessionExercises.Count(x => x.ExerciseId == request.ExerciseId);

            if (currentSetCountInSession >= programExercise!.Sets)
            {
                throw new InvalidOperationException($"Exercise ({request.ExerciseId}) for program {program.Id} is limited to {programExercise.Sets} sets. " + $"Session {session.Id} already has {currentSetCountInSession} sets.");
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