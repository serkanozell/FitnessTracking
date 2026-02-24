using BuildingBlocks.Domain.Exceptions;
using WorkoutPrograms.Domain.Events;

namespace WorkoutPrograms.Domain.Entity
{
    public class WorkoutProgram : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        private readonly List<WorkoutProgramSplit> _splits = new();
        public IReadOnlyList<WorkoutProgramSplit> Splits => _splits.AsReadOnly();

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

            workoutProgram.AddDomainEvent(new WorkoutProgramCreatedEvent(workoutProgram.Id));

            return workoutProgram;
        }

        public void Update(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;

            AddDomainEvent(new WorkoutProgramUpdatedEvent(Id));
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new WorkoutProgramActivatedEvent(Id));
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;

            AddDomainEvent(new WorkoutProgramDeletedEvent(Id));
        }

        public WorkoutProgramSplit AddSplit(string name, int order)
        {
            if (Splits.Any(x => x.Name == name))
            {
                throw new BusinessRuleViolationException(
                    $"Split with name ({name}) already exists in workout program {Id}.");
            }

            var split = new WorkoutProgramSplit(Guid.NewGuid(), Id, name, order);
            _splits.Add(split);

            AddDomainEvent(new WorkoutProgramSplitChangedEvent(Id));

            return split;
        }

        public void ActivateSplit(Guid splitId)
        {
            if (!IsActive || IsDeleted)
            {
                throw new BusinessRuleViolationException($"Workout program ({Id}) is not active.");
            }
            var split = Splits.SingleOrDefault(x => x.Id == splitId)
                        ?? throw new DomainNotFoundException("Split", splitId, "WorkoutProgram", Id);

            split.Activate();

            AddDomainEvent(new WorkoutProgramSplitChangedEvent(Id));
        }

        public void ActivateSplitExercise(Guid splitId, Guid workoutSplitExerciseId)
        {
            if (!IsActive || IsDeleted)
            {
                throw new BusinessRuleViolationException($"Workout program ({Id}) is not active.");
            }

            var split = Splits.SingleOrDefault(x => x.Id == splitId) ?? throw new DomainNotFoundException("Split", splitId, "WorkoutProgram", Id);

            if (!split.IsActive || split.IsDeleted)
            {
                throw new BusinessRuleViolationException($"Split ({splitId}) is not active in program {Id}.");
            }

            split.ActivateExercise(workoutSplitExerciseId);

            AddDomainEvent(new SplitExerciseChangedEvent(Id, splitId));
        }

        public void UpdateSplit(Guid splitId, string name, int order)
        {
            var split = Splits.SingleOrDefault(x => x.Id == splitId)
                        ?? throw new DomainNotFoundException(
                            "Split", splitId, "WorkoutProgram", Id);

            split.Update(name, order);

            AddDomainEvent(new WorkoutProgramSplitChangedEvent(Id));
        }

        public void RemoveSplit(Guid splitId)
        {
            var split = Splits.SingleOrDefault(x => x.Id == splitId);
            if (split is null)
            {
                return;
            }

            _splits.Remove(split);

            AddDomainEvent(new WorkoutProgramSplitChangedEvent(Id));
        }

        public bool ContainsExercise(Guid exerciseId)
        {
            return Splits.SelectMany(s => s.Exercises)
                         .Any(x => x.ExerciseId == exerciseId);
        }

        public WorkoutSplitExercise AddExerciseToSplit(Guid splitId, Guid exerciseId, int sets, int minimumReps, int maximumReps)
        {
            var split = Splits.SingleOrDefault(x => x.Id == splitId)
                        ?? throw new DomainNotFoundException("Split", splitId, "WorkoutProgram", Id);

            var exercise = split.AddExercise(exerciseId, sets, minimumReps, maximumReps);

            AddDomainEvent(new SplitExerciseChangedEvent(Id, splitId));

            return exercise;
        }

        public void UpdateExerciseInSplit(Guid splitId, Guid exerciseId, int sets, int minimumReps, int maximumReps)
        {
            var split = Splits.SingleOrDefault(x => x.Id == splitId)
                        ?? throw new DomainNotFoundException("Split", splitId, "WorkoutProgram", Id);

            split.UpdateExercise(exerciseId, sets, minimumReps, maximumReps);

            AddDomainEvent(new SplitExerciseChangedEvent(Id, splitId));
        }

        public void RemoveExerciseFromSplit(Guid splitId, Guid exerciseId)
        {
            var split = Splits.SingleOrDefault(x => x.Id == splitId)
                        ?? throw new DomainNotFoundException("Split", splitId, "WorkoutProgram", Id);

            split.RemoveExercise(exerciseId);

            AddDomainEvent(new SplitExerciseChangedEvent(Id, splitId));
        }
    }
}