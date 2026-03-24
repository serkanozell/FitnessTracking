using BuildingBlocks.Application.Abstractions;
using BodyMetrics.Contracts;
using Dashboard.Application.Dtos;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Dashboard.GetDashboard
{
    internal sealed class GetDashboardQueryHandler(IWorkoutProgramModule _programModule,
                                                   IWorkoutSessionModule _sessionModule,
                                                   IBodyMetricModule _bodyMetricModule,
                                                   ICurrentUser _currentUser) : IQueryHandler<GetDashboardQuery, Result<DashboardDto>>
    {
        public async Task<Result<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);
            var now = DateTime.Today;
            var weekStart = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
            if (weekStart > now) weekStart = weekStart.AddDays(-7);

            // Parallel fetch
            var activeProgramTask = _programModule.GetActiveProgramByUserAsync(userId, cancellationToken);
            var latestMetricTask = _bodyMetricModule.GetLatestByUserAsync(userId, cancellationToken);

            var weeklyStatsTask = await _sessionModule.GetStatsByUserAsync(userId, weekStart, now.AddDays(1), cancellationToken);
            var allTimeStatsTask = await _sessionModule.GetStatsByUserAsync(userId, DateTime.MinValue, now.AddDays(1), cancellationToken);

            await Task.WhenAll(activeProgramTask, latestMetricTask);

            var activeProgram = await activeProgramTask;
            var latestMetric = await latestMetricTask;
            var weeklyStats = weeklyStatsTask;
            var allTimeStats = allTimeStatsTask;            

            var dashboard = new DashboardDto
            {
                ActiveProgram = activeProgram is not null
                    ? new ActiveProgramDto
                    {
                        Id = activeProgram.Id,
                        Name = activeProgram.Name,
                        DayCount = (now - activeProgram.StartDate.Date).Days + 1,
                        CompletionPercentage = CalculateCompletion(activeProgram, now)
                    }
                    : null,
                WeeklyWorkouts = new WeeklyWorkoutsDto
                {
                    Completed = weeklyStats.SessionCount,
                    StreakDays = weeklyStats.StreakDays
                },
                LatestBodyMetric = latestMetric is not null
                    ? new LatestBodyMetricDto
                    {
                        Date = latestMetric.Date,
                        Weight = latestMetric.Weight,
                        BodyFatPercentage = latestMetric.BodyFatPercentage,
                        MuscleMass = latestMetric.MuscleMass
                    }
                    : null,
                Stats = new WorkoutStatsDto
                {
                    TotalWorkouts = allTimeStats.SessionCount,
                    TotalSets = allTimeStats.TotalSets,
                    TotalReps = allTimeStats.TotalReps
                }
            };

            return dashboard;
        }

        private static double CalculateCompletion(ActiveProgramInfo program, DateTime today)
        {
            var totalDays = (program.EndDate.Date - program.StartDate.Date).Days;
            if (totalDays <= 0) return 100;

            var elapsed = (today - program.StartDate.Date).Days;
            return Math.Min(100, Math.Round((double)elapsed / totalDays * 100, 1));
        }
    }
}