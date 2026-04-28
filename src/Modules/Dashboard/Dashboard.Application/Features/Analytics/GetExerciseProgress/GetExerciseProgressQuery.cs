using Dashboard.Application.Dtos;

namespace Dashboard.Application.Features.Analytics.GetExerciseProgress;

public sealed record GetExerciseProgressQuery(Guid ExerciseId, int Days = 90)
    : IQuery<Result<IReadOnlyList<ExerciseProgressPointDto>>>;
