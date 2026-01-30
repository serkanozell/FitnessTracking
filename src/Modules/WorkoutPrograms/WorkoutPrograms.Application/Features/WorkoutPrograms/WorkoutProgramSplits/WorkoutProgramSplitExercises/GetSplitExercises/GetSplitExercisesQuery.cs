using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises
{
    public sealed class GetSplitExercisesQuery : IQuery<IReadOnlyList<WorkoutProgramSplitExerciseDto>>
    {
        public Guid WorkoutProgramId { get; init; }
        public Guid WorkoutSplitId { get; set; }
    }
}