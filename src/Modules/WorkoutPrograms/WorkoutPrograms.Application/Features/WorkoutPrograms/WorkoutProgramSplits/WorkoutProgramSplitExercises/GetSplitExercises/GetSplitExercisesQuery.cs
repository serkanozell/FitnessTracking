using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises
{
    public sealed record GetSplitExercisesQuery(Guid WorkoutProgramId,
                                                Guid WorkoutSplitId) : IQuery<Result<IReadOnlyList<WorkoutProgramSplitExerciseDto>>>;
}