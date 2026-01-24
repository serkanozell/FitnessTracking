namespace FitnessTracking.Domain.Entity
{
    public class WorkoutProgram : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<WorkoutProgramExercise> Exercises { get; private set; } = new();

        private WorkoutProgram() { }

        public WorkoutProgram(Guid id, string name, DateTime startDate, DateTime endDate)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Update(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public WorkoutProgramExercise AddExercise(Guid exerciseId, int sets, int targetReps)
        {
            if (Exercises.Any(x => x.ExerciseId == exerciseId))
            {
                throw new InvalidOperationException(
                    $"Exercise ({exerciseId}) is already part of workout program {Id}.");
            }

            var workoutProgramExercise = new WorkoutProgramExercise(Guid.NewGuid(),
                                                                    exerciseId,
                                                                    sets,
                                                                    targetReps);

            Exercises.Add(workoutProgramExercise);

            return workoutProgramExercise;
        }

        public void UpdateExercise(Guid workoutProgramExerciseId, int sets, int targetReps)
        {
            var item = Exercises.SingleOrDefault(x => x.Id == workoutProgramExerciseId)
                       ?? throw new KeyNotFoundException(
                           $"WorkoutProgramExercise ({workoutProgramExerciseId}) not found in program {Id}.");

            item.Update(sets, targetReps);
        }

        public void RemoveExercise(Guid workoutProgramExerciseId)
        {
            var item = Exercises.SingleOrDefault(x => x.Id == workoutProgramExerciseId);
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