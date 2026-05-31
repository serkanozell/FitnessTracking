using Dashboard.Application.Dtos;

namespace Dashboard.Application.Features.Analytics.GetAnalyticsPage;

// User-scoped (filters by current user); intentionally NOT cacheable.
public sealed record GetAnalyticsPageQuery(int Days = 30,
                                           GroupingPeriodDto Period = GroupingPeriodDto.Day,
                                           Guid? ExerciseId = null,
                                           Guid? ProgramId = null,
                                           Guid? SplitId = null)
    : IQuery<Result<AnalyticsPageDto>>;
