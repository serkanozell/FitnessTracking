namespace FitnessTracking.Domain.Entity
{
    public class WorkoutProgram : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<WorkoutProgramExercise> WorkoutProgramExercises { get; private set; } = new();

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
            if (WorkoutProgramExercises.Any(x => x.ExerciseId == exerciseId))
            {
                throw new InvalidOperationException(
                    $"Exercise ({exerciseId}) is already part of workout program {Id}.");
            }

            var workoutProgramExercise = new WorkoutProgramExercise(Guid.NewGuid(),
                                                                    exerciseId,
                                                                    sets,
                                                                    targetReps);

            WorkoutProgramExercises.Add(workoutProgramExercise);

            return workoutProgramExercise;
        }

        public void UpdateExercise(Guid workoutProgramExerciseId, int sets, int targetReps)
        {
            var item = WorkoutProgramExercises.SingleOrDefault(x => x.Id == workoutProgramExerciseId)
                       ?? throw new KeyNotFoundException(
                           $"WorkoutProgramExercise ({workoutProgramExerciseId}) not found in program {Id}.");

            item.Update(sets, targetReps);
        }

        public void RemoveExercise(Guid workoutProgramExerciseId)
        {
            var item = WorkoutProgramExercises.SingleOrDefault(x => x.Id == workoutProgramExerciseId);
            if (item is null)
            {
                return;
            }

            WorkoutProgramExercises.Remove(item);
        }

        public bool ContainsExercise(Guid exerciseId)
        {
            return WorkoutProgramExercises.Any(x => x.ExerciseId == exerciseId);
        }
    }
}