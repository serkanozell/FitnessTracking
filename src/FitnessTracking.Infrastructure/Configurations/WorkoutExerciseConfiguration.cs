using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

//public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
//{
//    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
//    {
//        builder.ToTable("WorkoutExercises");

//        builder.HasKey(x => x.Id);

//        builder.Property(x => x.ExerciseId)
//               .IsRequired();

//        builder.Property(x => x.SetNumber)
//               .IsRequired();

//        builder.Property(x => x.Weight)
//               .HasPrecision(5, 2)
//               .IsRequired();

//        builder.Property(x => x.Reps)
//               .IsRequired();

//        builder.HasIndex(x => new { x.ExerciseId, x.SetNumber });
//    }
//}
