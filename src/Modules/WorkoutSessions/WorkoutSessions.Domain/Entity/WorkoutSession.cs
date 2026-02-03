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


            //workoutSession.AddDomainEvent(new WorkoutSessionCreatedDomainEvent(workoutSession.Id, workoutProgramId, dateTime));

            return workoutSession;
        }

        public SessionExercise AddEntry(Guid exerciseId, int setNumber, decimal weight, int reps)
        {
            // Aynı Exercise + SetNumber tekrar eklenmesin
            if (SessionExercises.Any(x => x.ExerciseId == exerciseId && x.SetNumber == setNumber))
            {
                throw new InvalidOperationException(
                    $"Set {setNumber} for exercise ({exerciseId}) already exists in session {Id}.");
            }

            var sessionExercise = new SessionExercise(Guid.NewGuid(),
                                                      exerciseId,
                                                      setNumber,
                                                      weight,
                                                      reps);

            SessionExercises.Add(sessionExercise);

            return sessionExercise;
        }

        public void UpdateDate(DateTime date)
        {
            Date = date;
        }

        public void UpdateEntry(Guid sessionExerciseId, int setNumber, decimal weight, int reps)
        {
            var entry = SessionExercises.FirstOrDefault(x => x.Id == sessionExerciseId) ?? throw new KeyNotFoundException($"SessionExercise ({sessionExerciseId}) not found in session {Id}.");

            entry.Update(setNumber, weight, reps);
        }

        public void RemoveEntry(Guid sessionExerciseId)
        {
            var entry = SessionExercises.FirstOrDefault(x => x.Id == sessionExerciseId);
            if (entry is null)
            {
                return;
            }

            SessionExercises.Remove(entry);
        }

        // İsteğe bağlı: ExerciseId + SetNumber ile güncelle/sil convenience metotları

        //public void UpdateEntry(Guid exerciseId, int setNumber, decimal weight, int reps)
        //{
        //    var entry = WorkoutExercises.FirstOrDefault(x =>
        //                     x.ExerciseId == exerciseId && x.SetNumber == setNumber)
        //                ?? throw new KeyNotFoundException(
        //                    $"WorkoutExercise with ExerciseId ({exerciseId}) and SetNumber ({setNumber}) not found in session {Id}.");

        //    entry.Update(setNumber, weight, reps);
        //}

        public void RemoveEntry(Guid exerciseId, int setNumber)
        {
            var entry = SessionExercises.FirstOrDefault(x =>
                x.ExerciseId == exerciseId && x.SetNumber == setNumber);

            if (entry is null)
            {
                return;
            }

            SessionExercises.Remove(entry);
        }
    }
}