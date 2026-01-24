namespace FitnessTracking.Domain.Entity
{
    public class Exercise : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string MuscleGroup { get; private set; }
        public string Description { get; private set; }

        private Exercise() { }

        public Exercise(Guid id, string name, string muscleGroup, string description)
        {
            Id = id;
            Name = name;
            MuscleGroup = muscleGroup;
            Description = description;
        }

        public void Update(string name, string muscleGroup, string description)
        {
            Name = name;
            MuscleGroup = muscleGroup;
            Description = description;
        }
    }
}