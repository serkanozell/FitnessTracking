using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class AnalyticsController(IDashboardService dashboardService,
                                 IExercisesService exercisesService) : Controller
{
    public async Task<IActionResult> Index(int days = 30,
                                           AnalyticsGroupingPeriod period = AnalyticsGroupingPeriod.Day,
                                           Guid? exerciseId = null,
                                           CancellationToken cancellationToken = default)
    {
        // Clamp days into reasonable range to keep aggregate queries fast.
        if (days <= 0) days = 30;
        if (days > 365) days = 365;

        // Fan out all independent dashboard/exercise lookups in parallel.
        var exercisesPagedTask = exercisesService.GetPagedAsync(1, 200, cancellationToken);
        var volumeTrendTask = dashboardService.GetVolumeTrendAsync(days, period, cancellationToken);
        var muscleDistributionTask = dashboardService.GetMuscleGroupDistributionAsync(days, cancellationToken);
        var personalRecordsTask = dashboardService.GetPersonalRecordsAsync(10, cancellationToken);

        await Task.WhenAll(exercisesPagedTask, volumeTrendTask, muscleDistributionTask, personalRecordsTask);

        var exercises = exercisesPagedTask.Result.Items.Where(e => e.IsActive && !e.IsDeleted).ToList();

        IReadOnlyList<ExerciseProgressPointDto> exerciseProgress = [];
        var selectedExerciseId = exerciseId ?? exercises.FirstOrDefault()?.Id;
        if (selectedExerciseId is not null && selectedExerciseId != Guid.Empty)
        {
            exerciseProgress = await dashboardService.GetExerciseProgressAsync(selectedExerciseId.Value,
                                                                              Math.Max(days, 90),
                                                                              cancellationToken);
        }

        var model = new AnalyticsViewModel
        {
            Days = days,
            Period = period,
            ExerciseId = selectedExerciseId,
            Exercises = exercises,
            VolumeTrend = volumeTrendTask.Result,
            MuscleGroupDistribution = muscleDistributionTask.Result,
            ExerciseProgress = exerciseProgress,
            PersonalRecords = personalRecordsTask.Result
        };

        return View(model);
    }
}
