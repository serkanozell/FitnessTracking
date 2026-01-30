namespace WorkoutPrograms.Domain.Entity
{
    public class WorkoutProgram : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<WorkoutProgramSplit> Splits { get; set; } = new();

        private WorkoutProgram() { }

        public WorkoutProgram(Guid id, string name, DateTime startDate, DateTime endDate)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public static WorkoutProgram Create(string name, DateTime startDate, DateTime endDate)
        {
            var workoutProgram = new WorkoutProgram(Guid.NewGuid(), name, startDate, endDate);

            //workoutProgram.AddDomainEvent(new WorkoutProgramCreatedDomainEvent(workoutProgram.Id));
            return workoutProgram;
        }

        public void Update(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public WorkoutProgramSplit AddSplit(string name, int order)
        {
            if (Splits.Any(x => x.Name == name))
            {
                throw new InvalidOperationException(
                    $"Split with name ({name}) already exists in workout program {Id}.");
            }

            var split = new WorkoutProgramSplit(Guid.NewGuid(), Id, name, order);
            Splits.Add(split);
            return split;
        }

        public void UpdateSplit(Guid splitId, string name, int order)
        {
            var split = Splits.SingleOrDefault(x => x.Id == splitId)
                        ?? throw new KeyNotFoundException(
                            $"Split ({splitId}) not found in program {Id}.");

            split.Update(name, order);
        }

        public void RemoveSplit(Guid splitId)
        {
            var split = Splits.SingleOrDefault(x => x.Id == splitId);
            if (split is null)
            {
                return;
            }

            Splits.Remove(split);
        }

        public bool ContainsExercise(Guid exerciseId)
        {
            return Splits.SelectMany(s => s.Exercises)
                         .Any(x => x.ExerciseId == exerciseId);
        }
    }
}