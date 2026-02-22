using Exercises.Domain.Events;

namespace Exercises.Domain.Entity
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

        public static Exercise Create(string name, string muscleGroup, string description)
        {
            var exercise = new Exercise(Guid.NewGuid(), name, muscleGroup, description);

            exercise.AddDomainEvent(new ExerciseCreatedEvent(exercise.Id));

            return exercise;
        }

        public void Update(string name, string muscleGroup, string description)
        {
            Name = name;
            MuscleGroup = muscleGroup;
            Description = description;

            AddDomainEvent(new ExerciseUpdatedEvent(Id));
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new ExerciseActivatedEvent(Id));
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;

            AddDomainEvent(new ExerciseDeletedEvent(Id));
        }
    }
}