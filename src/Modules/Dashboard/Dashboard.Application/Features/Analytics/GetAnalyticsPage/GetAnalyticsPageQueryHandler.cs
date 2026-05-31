using BuildingBlocks.Application.Abstractions;
using Dashboard.Application.Dtos;
using Exercises.Contracts;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Analytics.GetAnalyticsPage
{
    internal sealed class GetAnalyticsPageQueryHandler(IWorkoutSessionModule _sessionModule,
                                                       IExerciseModule _exerciseModule,
                                                       IWorkoutProgramModule _programModule,
                                                       ICurrentUser _currentUser)
        : IQueryHandler<GetAnalyticsPageQuery, Result<AnalyticsPageDto>>
    {
        private const int PersonalRecordsTop = 10;

        public async Task<Result<AnalyticsPageDto>> Handle(GetAnalyticsPageQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var days = request.Days;
            if (days <= 0) days = 30;
            if (days > 365) days = 365;

            var dateTo = DateTime.Today.AddDays(1);
            var dateFrom = DateTime.Today.AddDays(-days);

            // Load shared lookups once (exercises table was previously loaded 3x across calls).
            var exercises = await _exerciseModule.GetExercisesAsync(cancellationToken);
            var programs = await _programModule.GetProgramsByUserWithSplitsAsync(userId, cancellationToken);

            var exerciseById = exercises.ToDictionary(e => e.Id);

            var availableExercises = exercises
                .Where(e => e.IsActive && !e.IsDeleted)
                .ToList();

            // Validate program/split selection (drop invalid values silently).
            var selectedProgram = request.ProgramId.HasValue
                ? programs.FirstOrDefault(p => p.Id == request.ProgramId.Value && !p.IsDeleted)
                : null;
            var effectiveProgramId = selectedProgram?.Id;

            UserWorkoutProgramSplitInfo? selectedSplit = null;
            Guid? effectiveSplitId = null;
            if (selectedProgram is not null && request.SplitId.HasValue)
            {
                var match = selectedProgram.Splits.FirstOrDefault(s => s.Id == request.SplitId.Value && !s.IsDeleted);
                if (match is not null)
                {
                    selectedSplit = match;
                    effectiveSplitId = match.Id;
                }
            }

            // When a split is selected, restrict the exercise picker to the exercises
            // belonging to that split.
            if (selectedSplit is not null)
            {
                var splitExerciseIds = selectedSplit.Exercises
                    .Where(e => e.IsActive && !e.IsDeleted)
                    .Select(e => e.ExerciseId)
                    .ToHashSet();

                availableExercises = availableExercises
                    .Where(e => splitExerciseIds.Contains(e.Id))
                    .ToList();
            }

            // Keep the requested exercise only if it is part of the (possibly filtered)
            // exercise list; otherwise fall back to the first available exercise.
            var selectedExerciseId = request.ExerciseId.HasValue && availableExercises.Any(e => e.Id == request.ExerciseId.Value)
                ? request.ExerciseId
                : availableExercises.FirstOrDefault()?.Id;

            var period = (GroupingPeriod)request.Period;

            // Volume trend (program/split aware).
            var volumeData = await _sessionModule.GetVolumeTrendAsync(userId,
                                                                      dateFrom,
                                                                      dateTo,
                                                                      period,
                                                                      effectiveProgramId,
                                                                      effectiveSplitId,
                                                                      cancellationToken);

            // Muscle group distribution (reuses already-loaded exercises).
            var breakdown = await _sessionModule.GetExerciseVolumeBreakdownAsync(userId, dateFrom, dateTo, cancellationToken);
            var muscleDistribution = BuildMuscleDistribution(breakdown, exerciseById);

            // Personal records (reuses already-loaded exercises).
            var prs = await _sessionModule.GetPersonalRecordsAsync(userId, PersonalRecordsTop, cancellationToken);
            var personalRecords = BuildPersonalRecords(prs, exerciseById);

            // Exercise progress for the selected exercise.
            IReadOnlyList<ExerciseProgressPointDto> exerciseProgress = [];
            if (selectedExerciseId is not null && selectedExerciseId != Guid.Empty)
            {
                var progressData = await _sessionModule.GetExerciseProgressAsync(userId,
                                                                                 selectedExerciseId.Value,
                                                                                 DateTime.Today.AddDays(-Math.Max(days, 90)),
                                                                                 dateTo,
                                                                                 cancellationToken);

                exerciseProgress = progressData.Select(p => new ExerciseProgressPointDto
                {
                    Date = p.Date,
                    MaxWeight = p.MaxWeight,
                    MaxReps = p.MaxReps,
                    TotalVolume = p.TotalVolume,
                    Estimated1Rm = p.Estimated1Rm
                }).ToList();
            }

            var dto = new AnalyticsPageDto
            {
                Days = days,
                Period = request.Period,
                ExerciseId = selectedExerciseId,
                ProgramId = effectiveProgramId,
                SplitId = effectiveSplitId,
                Exercises = availableExercises
                    .Select(e => new AnalyticsExerciseDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        PrimaryMuscleGroup = e.PrimaryMuscleGroup
                    })
                    .ToList(),
                Programs = programs
                    .Where(p => !p.IsDeleted)
                    .Select(p => new AnalyticsProgramDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Splits = p.Splits
                            .Select(s => new AnalyticsProgramSplitDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Order = s.Order,
                                IsDeleted = s.IsDeleted
                            })
                            .ToList()
                    })
                    .ToList(),
                VolumeTrend = volumeData.Select(p => new VolumeTrendPointDto
                {
                    Date = p.Date,
                    TotalVolume = p.TotalVolume,
                    SessionCount = p.SessionCount,
                    TotalSets = p.TotalSets,
                    TotalReps = p.TotalReps,
                    SplitOrders = p.SplitOrders
                }).ToList(),
                MuscleGroupDistribution = muscleDistribution,
                ExerciseProgress = exerciseProgress,
                PersonalRecords = personalRecords
            };

            return Result<AnalyticsPageDto>.Success(dto);
        }

        private static IReadOnlyList<MuscleGroupVolumeDto> BuildMuscleDistribution(
            IReadOnlyList<ExerciseVolumeInfo> breakdown,
            IReadOnlyDictionary<Guid, ExerciseInfo> exerciseById)
        {
            if (breakdown.Count == 0)
                return [];

            return breakdown
                .GroupBy(b => exerciseById.TryGetValue(b.ExerciseId, out var info) ? info.PrimaryMuscleGroup : "Unknown")
                .Select(g => new MuscleGroupVolumeDto
                {
                    MuscleGroup = g.Key,
                    TotalVolume = g.Sum(x => x.TotalVolume),
                    SetCount = g.Sum(x => x.SetCount),
                    TotalReps = g.Sum(x => x.TotalReps)
                })
                .OrderByDescending(x => x.TotalVolume)
                .ToList();
        }

        private static IReadOnlyList<PersonalRecordDto> BuildPersonalRecords(
            IReadOnlyList<PersonalRecordInfo> prs,
            IReadOnlyDictionary<Guid, ExerciseInfo> exerciseById)
        {
            if (prs.Count == 0)
                return [];

            return prs.Select(p =>
            {
                exerciseById.TryGetValue(p.ExerciseId, out var info);
                return new PersonalRecordDto
                {
                    ExerciseId = p.ExerciseId,
                    ExerciseName = info?.Name ?? "Unknown",
                    PrimaryMuscleGroup = info?.PrimaryMuscleGroup,
                    MaxWeight = p.MaxWeight,
                    RepsAtMaxWeight = p.RepsAtMaxWeight,
                    Estimated1Rm = p.Estimated1Rm,
                    AchievedOn = p.AchievedOn
                };
            }).ToList();
        }
    }
}
