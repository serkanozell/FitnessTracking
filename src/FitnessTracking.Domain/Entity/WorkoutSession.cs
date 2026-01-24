namespace FitnessTracking.Domain.Entity
{
    public class WorkoutSession : AggregateRoot<Guid>
    {
        public Guid WorkoutProgramId { get; private set; }
        public DateTime Date { get; private set; }

        public List<WorkoutExercise> WorkoutExercises { get; private set; } = new();

        private WorkoutSession() { }

        public WorkoutSession(Guid id, Guid workoutProgramId, DateTime date)
        {
            Id = id;
            WorkoutProgramId = workoutProgramId;
            Date = date;
        }

        public WorkoutExercise AddEntry(Guid exerciseId, int setNumber, decimal weight, int reps)
        {
            // Aynı Exercise + SetNumber tekrar eklenmesin
            if (WorkoutExercises.Any(x => x.ExerciseId == exerciseId && x.SetNumber == setNumber))
            {
                throw new InvalidOperationException(
                    $"Set {setNumber} for exercise ({exerciseId}) already exists in session {Id}.");
            }

            var workoutExercise = new WorkoutExercise(Guid.NewGuid(),
                                                      exerciseId,
                                                      setNumber,
                                                      weight,
                                                      reps);

            WorkoutExercises.Add(workoutExercise);

            return workoutExercise;
        }

        public void UpdateDate(DateTime date)
        {
            Date = date;
        }

        public void UpdateEntry(Guid workoutExerciseId, int setNumber, decimal weight, int reps)
        {
            var entry = WorkoutExercises.FirstOrDefault(x => x.Id == workoutExerciseId) ?? throw new KeyNotFoundException($"WorkoutExercise ({workoutExerciseId}) not found in session {Id}.");

            entry.Update(setNumber, weight, reps);
        }

        public void RemoveEntry(Guid workoutExerciseId)
        {
            var entry = WorkoutExercises.FirstOrDefault(x => x.Id == workoutExerciseId);
            if (entry is null)
            {
                return;
            }

            WorkoutExercises.Remove(entry);
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
            var entry = WorkoutExercises.FirstOrDefault(x =>
                x.ExerciseId == exerciseId && x.SetNumber == setNumber);

            if (entry is null)
            {
                return;
            }

            WorkoutExercises.Remove(entry);
        }
    }
}