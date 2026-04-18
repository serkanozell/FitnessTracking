using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using WorkoutSessions.Domain.Events;

namespace WorkoutSessions.Domain.Entity
{
    public class WorkoutSession : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid WorkoutProgramId { get; private set; }
        public Guid WorkoutProgramSplitId { get; private set; }
        public DateTime Date { get; private set; }

        private readonly List<SessionExercise> _sessionExercises = new();
        public IReadOnlyList<SessionExercise> SessionExercises => _sessionExercises.AsReadOnly();

        private WorkoutSession() { }

        private WorkoutSession(Guid id, Guid userId, Guid workoutProgramId, Guid workoutProgramSplitId, DateTime date)
        {
            Id = id;
            UserId = userId;
            WorkoutProgramId = workoutProgramId;
            WorkoutProgramSplitId = workoutProgramSplitId;
            Date = date;
        }

        public static WorkoutSession Create(Guid userId, Guid workoutProgramId, Guid workoutProgramSplitId, DateTime dateTime)
        {
            var workoutSession = new WorkoutSession(Guid.NewGuid(), userId, workoutProgramId, workoutProgramSplitId, dateTime);

            workoutSession.AddDomainEvent(new WorkoutSessionCreatedEvent(workoutSession.Id, workoutProgramId, workoutProgramSplitId));

            return workoutSession;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new WorkoutSessionActivatedEvent(Id));
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;

            foreach (var exercise in _sessionExercises)
            {
                exercise.Delete();
            }

            AddDomainEvent(new WorkoutSessionDeletedEvent(Id, WorkoutProgramId));
        }

        public SessionExercise AddEntry(Guid exerciseId, int setNumber, decimal weight, int reps)
        {
            if (SessionExercises.Any(x => x.ExerciseId == exerciseId && x.SetNumber == setNumber))
            {
                throw new BusinessRuleViolationException(
                    $"Set {setNumber} for exercise ({exerciseId}) already exists in session {Id}.");
            }

            var sessionExercise = new SessionExercise(Guid.NewGuid(),
                                                      exerciseId,
                                                      setNumber,
                                                      weight,
                                                      reps);

            _sessionExercises.Add(sessionExercise);

            AddDomainEvent(new SessionExerciseChangedEvent(Id));

            return sessionExercise;
        }

        public void ActivateEntry(Guid sessionExerciseId)
        {
            if (!IsActive || IsDeleted)
            {
                throw new BusinessRuleViolationException($"The WorkoutSession ({Id}) is not active.");
            }

            var entry = SessionExercises.FirstOrDefault(x => x.Id == sessionExerciseId) ?? throw new DomainNotFoundException("SessionExercise", sessionExerciseId, "WorkoutSession", Id);

            entry.Activate();

            AddDomainEvent(new SessionExerciseChangedEvent(Id));
        }

        public void UpdateDate(DateTime date)
        {
            Date = date;

            AddDomainEvent(new WorkoutSessionUpdatedEvent(Id, WorkoutProgramId));
        }

        public void UpdateEntry(Guid sessionExerciseId, int setNumber, decimal weight, int reps)
        {
            var entry = SessionExercises.FirstOrDefault(x => x.Id == sessionExerciseId) ?? throw new DomainNotFoundException("SessionExercise", sessionExerciseId, "WorkoutSession", Id);

            entry.Update(setNumber, weight, reps);

            AddDomainEvent(new SessionExerciseChangedEvent(Id));
        }

        public void RemoveEntry(Guid sessionExerciseId)
        {
            var entry = SessionExercises.FirstOrDefault(x => x.Id == sessionExerciseId);
            if (entry is null)
            {
                return;
            }

            _sessionExercises.Remove(entry);

            AddDomainEvent(new SessionExerciseChangedEvent(Id));
        }

        public void RemoveEntry(Guid exerciseId, int setNumber)
        {
            var entry = SessionExercises.FirstOrDefault(x =>
                x.ExerciseId == exerciseId && x.SetNumber == setNumber);

            if (entry is null)
            {
                return;
            }

            _sessionExercises.Remove(entry);

            AddDomainEvent(new SessionExerciseChangedEvent(Id));
        }
    }
}