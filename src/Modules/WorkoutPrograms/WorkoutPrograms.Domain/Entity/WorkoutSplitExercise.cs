namespace WorkoutPrograms.Domain.Entity
{
    public class WorkoutSplitExercise : Entity<Guid>
    {
        public Guid ExerciseId { get; private set; }
        public Guid WorkoutProgramSplitId { get; private set; }
        public int Sets { get; private set; }
        public int MinimumReps { get; private set; }
        public int MaximumReps { get; private set; }

        private WorkoutSplitExercise() { }

        public WorkoutSplitExercise(Guid id,
                                      Guid exerciseId,
                                      int sets,
                                      int minimumReps,
                                      int maximumReps,
                                      Guid workoutProgramSplitId)
        {
            Id = id;
            ExerciseId = exerciseId;
            Sets = sets;
            MinimumReps = minimumReps;
            MaximumReps = maximumReps;
            WorkoutProgramSplitId = workoutProgramSplitId;
        }

        public void Update(int sets, int minimumReps, int maximumReps)
        {
            Sets = sets;
            MinimumReps = minimumReps;
            MaximumReps = maximumReps;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }
    }
}