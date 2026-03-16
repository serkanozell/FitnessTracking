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