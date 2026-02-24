using Exercises.Domain.Enums;
using Exercises.Domain.Events;

namespace Exercises.Domain.Entity
{
    public class Exercise : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public MuscleGroup PrimaryMuscleGroup { get; private set; }
        public MuscleGroup? SecondaryMuscleGroup { get; private set; }
        public string Description { get; private set; }

        private Exercise() { }

        public Exercise(Guid id, string name, MuscleGroup primaryMuscleGroup, MuscleGroup? secondaryMuscleGroup, string description)
        {
            Id = id;
            Name = name;
            PrimaryMuscleGroup = primaryMuscleGroup;
            SecondaryMuscleGroup = secondaryMuscleGroup;
            Description = description;
        }

        public static Exercise Create(string name, MuscleGroup primaryMuscleGroup, MuscleGroup? secondaryMuscleGroup, string description)
        {
            var exercise = new Exercise(Guid.NewGuid(), name, primaryMuscleGroup, secondaryMuscleGroup, description);

            exercise.AddDomainEvent(new ExerciseCreatedEvent(exercise.Id));

            return exercise;
        }

        public void Update(string name, MuscleGroup primaryMuscleGroup, MuscleGroup? secondaryMuscleGroup, string description)
        {
            Name = name;
            PrimaryMuscleGroup = primaryMuscleGroup;
            SecondaryMuscleGroup = secondaryMuscleGroup;
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