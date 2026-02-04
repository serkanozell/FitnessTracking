namespace WorkoutPrograms.Domain.Entity
{
    public class WorkoutSplitExercise : Entity<Guid>
    {
        public Guid ExerciseId { get; private set; }
        public Guid WorkoutProgramSplitId { get; private set; }
        public int Sets { get; private set; }
        public int TargetReps { get; private set; }

        private WorkoutSplitExercise() { }

        public WorkoutSplitExercise(Guid id,
                                      Guid exerciseId,
                                      int sets,
                                      int targetReps,
                                      Guid workoutProgramSplitId)
        {
            Id = id;
            ExerciseId = exerciseId;
            Sets = sets;
            TargetReps = targetReps;
            WorkoutProgramSplitId = workoutProgramSplitId;
        }

        public void Update(int sets, int targetReps)
        {
            Sets = sets;
            TargetReps = targetReps;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }
    }
}