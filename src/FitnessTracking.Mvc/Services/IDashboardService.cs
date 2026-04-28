using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services;

public interface IDashboardService
{
    Task<DashboardDto?> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WeightTrendDto>> GetWeightTrendAsync(int days = 90, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VolumeTrendPointDto>> GetVolumeTrendAsync(int days = 30,
                                                                 AnalyticsGroupingPeriod period = AnalyticsGroupingPeriod.Day,
                                                                 CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExerciseProgressPointDto>> GetExerciseProgressAsync(Guid exerciseId,
                                                                           int days = 90,
                                                                           CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MuscleGroupVolumeDto>> GetMuscleGroupDistributionAsync(int days = 30,
                                                                              CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PersonalRecordDto>> GetPersonalRecordsAsync(int top = 10,
                                                                   CancellationToken cancellationToken = default);
}
