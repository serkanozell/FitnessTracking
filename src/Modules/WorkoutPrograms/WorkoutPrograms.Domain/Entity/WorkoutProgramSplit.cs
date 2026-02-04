namespace WorkoutPrograms.Domain.Entity
{
    public class WorkoutProgramSplit : Entity<Guid>
    {
        public Guid WorkoutProgramId { get; private set; }

        // Örn: "Gün 1", "Göğüs Günü", "Upper", "Lower"
        public string Name { get; private set; }

        // İstersen: Haftadaki sırası (1 = Pazartesi, vs) veya sadece program içi sıralama
        public int Order { get; private set; }

        public List<WorkoutSplitExercise> Exercises { get; private set; } = new();

        private WorkoutProgramSplit() { }

        public WorkoutProgramSplit(Guid id, Guid workoutProgramId, string name, int order)
        {
            Id = id;
            WorkoutProgramId = workoutProgramId;
            Name = name;
            Order = order;
        }

        public void Update(string name, int order)
        {
            Name = name;
            Order = order;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public WorkoutSplitExercise AddExercise(Guid exerciseId, int sets, int targetReps)
        {
            if (Exercises.Any(x => x.ExerciseId == exerciseId))
            {
                throw new InvalidOperationException(
                    $"Exercise ({exerciseId}) is already part of split {Id}.");
            }

            var workoutSplitExercise = new WorkoutSplitExercise(
                Guid.NewGuid(),
                exerciseId,
                sets,
                targetReps,
                Id // SplitId
            );

            Exercises.Add(workoutSplitExercise);

            return workoutSplitExercise;
        }

        public void UpdateExercise(Guid workoutSplitExerciseId, int sets, int targetReps)
        {
            var item = Exercises.SingleOrDefault(x => x.Id == workoutSplitExerciseId)
                       ?? throw new KeyNotFoundException(
                           $"WorkoutSplitExercise ({workoutSplitExerciseId}) not found in split {Id}.");

            item.Update(sets, targetReps);
        }

        public void ActivateExercise(Guid workoutSplitExerciseId)
        {
            var item = Exercises.SingleOrDefault(x => x.Id == workoutSplitExerciseId)
                       ?? throw new KeyNotFoundException($"WorkoutSplitExercise ({workoutSplitExerciseId}) not found in split {Id}.");

            item.Activate();
        }

        public void RemoveExercise(Guid workoutSplitExerciseId)
        {
            var item = Exercises.SingleOrDefault(x => x.Id == workoutSplitExerciseId);
            if (item is null)
            {
                return;
            }

            Exercises.Remove(item);
        }

        public bool ContainsExercise(Guid exerciseId)
        {
            return Exercises.Any(x => x.ExerciseId == exerciseId);
        }
    }
}