namespace FitnessTracking.Domain.Entity
{
    public class WorkoutProgramExercise : Entity<Guid>
    {
        public Guid ExerciseId { get; private set; }
        public int Sets { get; private set; }
        public int TargetReps { get; private set; }

        private WorkoutProgramExercise() { }

        public WorkoutProgramExercise(Guid id,
                                      Guid exerciseId,
                                      int sets,
                                      int targetReps)
        {
            Id = id;
            ExerciseId = exerciseId;
            Sets = sets;
            TargetReps = targetReps;
        }

        public void Update(int sets, int targetReps)
        {
            Sets = sets;
            TargetReps = targetReps;
        }
    }
}