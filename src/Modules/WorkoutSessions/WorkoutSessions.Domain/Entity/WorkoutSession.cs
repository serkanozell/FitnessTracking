using BuildingBlocks.Domain.Exceptions;
using WorkoutSessions.Domain.Events;

namespace WorkoutSessions.Domain.Entity
{
    public class WorkoutSession : AggregateRoot<Guid>
    {
        public Guid WorkoutProgramId { get; private set; }
        public DateTime Date { get; private set; }

        public List<SessionExercise> SessionExercises { get; private set; } = new();

        private WorkoutSession() { }

        public WorkoutSession(Guid id, Guid workoutProgramId, DateTime date)
        {
            Id = id;
            WorkoutProgramId = workoutProgramId;
            Date = date;
        }

        public static WorkoutSession Create(Guid workoutProgramId, DateTime dateTime)
        {
            var workoutSession = new WorkoutSession(Guid.NewGuid(), workoutProgramId, dateTime);

            workoutSession.AddDomainEvent(new WorkoutSessionCreatedEvent(workoutSession.Id, workoutProgramId));

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

            SessionExercises.Add(sessionExercise);

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

            SessionExercises.Remove(entry);

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

            SessionExercises.Remove(entry);

            AddDomainEvent(new SessionExerciseChangedEvent(Id));
        }
    }
}