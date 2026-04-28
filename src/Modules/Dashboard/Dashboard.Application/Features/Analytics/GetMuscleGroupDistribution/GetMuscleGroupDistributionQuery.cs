using Dashboard.Application.Dtos;

namespace Dashboard.Application.Features.Analytics.GetMuscleGroupDistribution;

public sealed record GetMuscleGroupDistributionQuery(int Days = 30)
    : IQuery<Result<IReadOnlyList<MuscleGroupVolumeDto>>>;
