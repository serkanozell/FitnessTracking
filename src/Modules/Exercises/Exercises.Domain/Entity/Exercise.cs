using BuildingBlocks.Domain.Abstractions;
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
        public string? ImageUrl { get; private set; }
        public string? VideoUrl { get; private set; }

        private Exercise() { }

        private Exercise(Guid id, string name, MuscleGroup primaryMuscleGroup, MuscleGroup? secondaryMuscleGroup, string description, string? imageUrl = null, string? videoUrl = null)
        {
            Id = id;
            Name = name;
            PrimaryMuscleGroup = primaryMuscleGroup;
            SecondaryMuscleGroup = secondaryMuscleGroup;
            Description = description;
            ImageUrl = imageUrl;
            VideoUrl = videoUrl;
        }

        public static Exercise Create(string name, MuscleGroup primaryMuscleGroup, MuscleGroup? secondaryMuscleGroup, string description, string? imageUrl = null, string? videoUrl = null)
        {
            var exercise = new Exercise(Guid.NewGuid(), name, primaryMuscleGroup, secondaryMuscleGroup, description, imageUrl, videoUrl);

            exercise.AddDomainEvent(new ExerciseCreatedEvent(exercise.Id));

            return exercise;
        }

        public void Update(string name, MuscleGroup primaryMuscleGroup, MuscleGroup? secondaryMuscleGroup, string description, string? imageUrl, string? videoUrl)
        {
            Name = name;
            PrimaryMuscleGroup = primaryMuscleGroup;
            SecondaryMuscleGroup = secondaryMuscleGroup;
            Description = description;
            ImageUrl = imageUrl;
            VideoUrl = videoUrl;

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