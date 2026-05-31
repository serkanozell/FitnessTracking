using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class AnalyticsController(IDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index(int days = 30,
                                           AnalyticsGroupingPeriod period = AnalyticsGroupingPeriod.Day,
                                           Guid? exerciseId = null,
                                           Guid? programId = null,
                                           Guid? splitId = null,
                                           CancellationToken cancellationToken = default)
    {
        // All analytics-page data is aggregated server-side in a single API call.
        // This avoids HTTP fan-out from MVC and repeated loading of the same
        // reference data (exercises/programs) across multiple handlers.
        var page = await dashboardService.GetAnalyticsPageAsync(days,
                                                                period,
                                                                exerciseId,
                                                                programId,
                                                                splitId,
                                                                cancellationToken);

        var model = page is null
            ? new AnalyticsViewModel { Days = days, Period = period }
            : new AnalyticsViewModel
            {
                Days = page.Days,
                Period = page.Period,
                ExerciseId = page.ExerciseId,
                ProgramId = page.ProgramId,
                SplitId = page.SplitId,
                Exercises = page.Exercises
                    .Select(e => new ExerciseDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        PrimaryMuscleGroup = e.PrimaryMuscleGroup
                    })
                    .ToList(),
                Programs = page.Programs
                    .Select(p => new WorkoutProgramDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Splits = p.Splits
                            .Select(s => new WorkoutProgramSplitDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Order = s.Order,
                                IsDeleted = s.IsDeleted
                            })
                            .ToList()
                    })
                    .ToList(),
                VolumeTrend = page.VolumeTrend,
                MuscleGroupDistribution = page.MuscleGroupDistribution,
                ExerciseProgress = page.ExerciseProgress,
                PersonalRecords = page.PersonalRecords
            };

        return View(model);
    }
}
