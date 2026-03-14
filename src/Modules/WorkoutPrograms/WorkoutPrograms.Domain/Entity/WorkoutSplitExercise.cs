using BuildingBlocks.Domain.Abstractions;
using WorkoutPrograms.Domain.ValueObjects;

namespace WorkoutPrograms.Domain.Entity
{
    public class WorkoutSplitExercise : Entity<Guid>
    {
        public Guid ExerciseId { get; private set; }
        public Guid WorkoutProgramSplitId { get; private set; }
        public int Sets { get; private set; }
        public RepRange RepRange { get; private set; }

        private WorkoutSplitExercise() { }

        public WorkoutSplitExercise(Guid id,
                                      Guid exerciseId,
                                      int sets,
                                      RepRange repRange,
                                      Guid workoutProgramSplitId)
        {
            Id = id;
            ExerciseId = exerciseId;
            Sets = sets;
            RepRange = repRange;
            WorkoutProgramSplitId = workoutProgramSplitId;
        }

        public void Update(int sets, RepRange repRange)
        {
            Sets = sets;
            RepRange = repRange;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }
    }
}