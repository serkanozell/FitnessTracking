using FitnessTracking.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

//public class WorkoutProgramExerciseConfiguration : IEntityTypeConfiguration<WorkoutProgramExercise>
//{
//    public void Configure(EntityTypeBuilder<WorkoutProgramExercise> builder)
//    {
//        builder.ToTable("WorkoutProgramExercises");

//        builder.HasKey(x => x.Id);

//        builder.Property(x => x.ExerciseId)
//               .IsRequired();

//        builder.Property(x => x.Sets)
//               .IsRequired();

//        builder.Property(x => x.TargetReps)
//               .IsRequired();

//        builder.Property(x => x.WorkoutProgramId)
//               .IsRequired();

//        // BaseEntity / audit alanları
//        //builder.Property(x => x.CreatedBy);
//        //builder.Property(x => x.CreatedDate);
//        //builder.Property(x => x.UpdatedBy);
//        //builder.Property(x => x.UpdatedDate);

//        builder.HasIndex(x => new { x.WorkoutProgramId, x.ExerciseId })
//               .IsUnique();
//    }
//}