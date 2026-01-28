public class SessionExercise : Entity<Guid>
{
    public Guid ExerciseId { get; private set; }
    public int SetNumber { get; private set; }
    public decimal Weight { get; private set; }
    public int Reps { get; private set; }

    private SessionExercise() { }

    public SessionExercise(Guid id, Guid exerciseId, int setNumber, decimal weight, int reps)
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