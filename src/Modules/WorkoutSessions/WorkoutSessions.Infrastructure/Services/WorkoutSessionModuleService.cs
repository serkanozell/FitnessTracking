using Microsoft.EntityFrameworkCore;
using WorkoutSessions.Contracts;
using WorkoutSessions.Infrastructure.Persistence;

namespace WorkoutSessions.Infrastructure.Services
{
    internal sealed class WorkoutSessionModuleService(WorkoutSessionsDbContext _context) : IWorkoutSessionModule
    {
        public async Task<WorkoutSessionStatsInfo> GetStatsByUserAsync(Guid userId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
        {
            var sessions = await _context.WorkoutSessions.Include(x => x.SessionExercises)
                                                         .Where(x => x.UserId == userId && x.IsActive && !x.IsDeleted && x.Date >= dateFrom && x.Date <= dateTo)
                                                         .AsNoTracking()
                                                         .ToListAsync(cancellationToken);

            var sessionCount = sessions.Count;
            var totalSets = sessions.SelectMany(s => s.SessionExercises).Count();
            var totalReps = sessions.SelectMany(s => s.SessionExercises).Sum(e => e.Reps);
            var streakDays = CalculateStreak(sessions.Select(s => s.Date).Distinct().OrderByDescending(d => d));

            return new WorkoutSessionStatsInfo(sessionCount, totalSets, totalReps, streakDays);
        }

        public async Task<IReadOnlyList<WorkoutVolumePoint>> GetVolumeTrendAsync(Guid userId,
                                                                                 DateTime dateFrom,
                                                                                 DateTime dateTo,
                                                                                 GroupingPeriod period,
                                                                                 CancellationToken cancellationToken = default)
        {
            // Per-session aggregation in SQL (one row per session in the range)
            var perSession = await _context.WorkoutSessions
                .AsNoTracking()
                .Where(s => s.UserId == userId
                            && s.IsActive
                            && !s.IsDeleted
                            && s.Date >= dateFrom
                            && s.Date <= dateTo)
                .Select(s => new
                {
                    s.Date,
                    Volume = s.SessionExercises
                              .Where(e => e.IsActive && !e.IsDeleted)
                              .Sum(e => (decimal?)e.Weight * e.Reps) ?? 0m,
                    SetCount = s.SessionExercises.Count(e => e.IsActive && !e.IsDeleted),
                    RepCount = s.SessionExercises.Where(e => e.IsActive && !e.IsDeleted).Sum(e => (int?)e.Reps) ?? 0
                })
                .ToListAsync(cancellationToken);

            return perSession
                .GroupBy(x => GetBucketStart(x.Date, period))
                .OrderBy(g => g.Key)
                .Select(g => new WorkoutVolumePoint(
                    Date: g.Key,
                    TotalVolume: g.Sum(x => x.Volume),
                    SessionCount: g.Count(),
                    TotalSets: g.Sum(x => x.SetCount),
                    TotalReps: g.Sum(x => x.RepCount)))
                .ToList();
        }

        public async Task<IReadOnlyList<ExerciseProgressPoint>> GetExerciseProgressAsync(Guid userId,
                                                                                         Guid exerciseId,
                                                                                         DateTime dateFrom,
                                                                                         DateTime dateTo,
                                                                                         CancellationToken cancellationToken = default)
        {
            // Flat projection of (session date, weight, reps) for the requested exercise
            var rows = await _context.WorkoutSessions
                .AsNoTracking()
                .Where(s => s.UserId == userId
                            && s.IsActive
                            && !s.IsDeleted
                            && s.Date >= dateFrom
                            && s.Date <= dateTo)
                .SelectMany(s => s.SessionExercises
                    .Where(e => e.IsActive && !e.IsDeleted && e.ExerciseId == exerciseId)
                    .Select(e => new { s.Date, e.Weight, e.Reps }))
                .ToListAsync(cancellationToken);

            return rows
                .GroupBy(r => r.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var maxWeight = g.Max(r => r.Weight);
                    var bestSet = g.OrderByDescending(r => Estimate1Rm(r.Weight, r.Reps)).First();
                    return new ExerciseProgressPoint(
                        Date: g.Key,
                        MaxWeight: maxWeight,
                        MaxReps: g.Max(r => r.Reps),
                        TotalVolume: g.Sum(r => r.Weight * r.Reps),
                        Estimated1Rm: Math.Round(Estimate1Rm(bestSet.Weight, bestSet.Reps), 2));
                })
                .ToList();
        }

        public async Task<IReadOnlyList<ExerciseVolumeInfo>> GetExerciseVolumeBreakdownAsync(Guid userId,
                                                                                             DateTime dateFrom,
                                                                                             DateTime dateTo,
                                                                                             CancellationToken cancellationToken = default)
        {
            var rows = await _context.WorkoutSessions
                .AsNoTracking()
                .Where(s => s.UserId == userId
                            && s.IsActive
                            && !s.IsDeleted
                            && s.Date >= dateFrom
                            && s.Date <= dateTo)
                .SelectMany(s => s.SessionExercises
                    .Where(e => e.IsActive && !e.IsDeleted)
                    .Select(e => new { e.ExerciseId, e.Weight, e.Reps }))
                .GroupBy(x => x.ExerciseId)
                .Select(g => new ExerciseVolumeInfo(
                    g.Key,
                    g.Sum(x => x.Weight * x.Reps),
                    g.Count(),
                    g.Sum(x => x.Reps)))
                .ToListAsync(cancellationToken);

            return rows
                .OrderByDescending(x => x.TotalVolume)
                .ToList();
        }

        public async Task<IReadOnlyList<PersonalRecordInfo>> GetPersonalRecordsAsync(Guid userId,
                                                                                     int top = 10,
                                                                                     CancellationToken cancellationToken = default)
        {
            var rows = await _context.WorkoutSessions
                .AsNoTracking()
                .Where(s => s.UserId == userId && s.IsActive && !s.IsDeleted)
                .SelectMany(s => s.SessionExercises
                    .Where(e => e.IsActive && !e.IsDeleted)
                    .Select(e => new { e.ExerciseId, e.Weight, e.Reps, s.Date }))
                .ToListAsync(cancellationToken);

            return rows
                .GroupBy(r => r.ExerciseId)
                .Select(g =>
                {
                    var best = g.OrderByDescending(r => Estimate1Rm(r.Weight, r.Reps)).First();
                    return new PersonalRecordInfo(
                        ExerciseId: g.Key,
                        MaxWeight: g.Max(r => r.Weight),
                        RepsAtMaxWeight: best.Reps,
                        Estimated1Rm: Math.Round(Estimate1Rm(best.Weight, best.Reps), 2),
                        AchievedOn: best.Date);
                })
                .OrderByDescending(x => x.Estimated1Rm)
                .Take(top)
                .ToList();
        }

        private static decimal Estimate1Rm(decimal weight, int reps)
        {
            // Epley formula: 1RM = weight × (1 + reps / 30)
            if (reps <= 1) return weight;
            return weight * (1m + reps / 30m);
        }

        private static DateTime GetBucketStart(DateTime date, GroupingPeriod period) => period switch
        {
            GroupingPeriod.Week => StartOfIsoWeek(date.Date),
            GroupingPeriod.Month => new DateTime(date.Year, date.Month, 1),
            _ => date.Date
        };

        private static DateTime StartOfIsoWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private static int CalculateStreak(IEnumerable<DateTime> datesDescending)
        {
            var streak = 0;
            var expected = DateTime.Today;

            foreach (var date in datesDescending)
            {
                if (date.Date == expected.Date)
                {
                    streak++;
                    expected = expected.AddDays(-1);
                }
                else if (date.Date < expected.Date)
                {
                    break;
                }
            }

            return streak;
        }
    }
}