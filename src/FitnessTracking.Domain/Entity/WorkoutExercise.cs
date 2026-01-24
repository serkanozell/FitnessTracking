public class WorkoutExercise : Entity<Guid>
{
    public Guid ExerciseId { get; private set; }
    public int SetNumber { get; private set; }
    public decimal Weight { get; private set; }
    public int Reps { get; private set; }

    private WorkoutExercise() { }

    public WorkoutExercise(Guid id, Guid exerciseId, int setNumber, decimal weight, int reps)
    {
        Id = id;
        ExerciseId = exerciseId;
        SetNumber = setNumber;
        Weight = weight;
        Reps = reps;
    }

    public void Update(int setNumber, decimal weight, int reps)
    {
        SetNumber = setNumber;
        Weight = weight;
        Reps = reps;
    }
}