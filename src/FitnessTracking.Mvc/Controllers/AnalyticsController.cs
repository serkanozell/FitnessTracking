using FitnessTracking.Mvc.Models;
using FitnessTracking.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Mvc.Controllers;

[Authorize]
public class AnalyticsController(IDashboardService dashboardService,
                                 IExercisesService exercisesService,
                                 IWorkoutProgramsService workoutProgramsService) : Controller
{
    public async Task<IActionResult> Index(int days = 30,
                                           AnalyticsGroupingPeriod period = AnalyticsGroupingPeriod.Day,
                                           Guid? exerciseId = null,
                                           Guid? programId = null,
                                           Guid? splitId = null,
                                           CancellationToken cancellationToken = default)
    {
        // Clamp days into reasonable range to keep aggregate queries fast.
        if (days <= 0) days = 30;
        if (days > 365) days = 365;

        // Fan out all independent dashboard/exercise lookups in parallel.
        var exercisesPagedTask = exercisesService.GetPagedAsync(1, 200, cancellationToken);
        var programsPagedTask = workoutProgramsService.GetPagedAsync(1, 200, cancellationToken);
        var muscleDistributionTask = dashboardService.GetMuscleGroupDistributionAsync(days, cancellationToken);
        var personalRecordsTask = dashboardService.GetPersonalRecordsAsync(10, cancellationToken);

        await Task.WhenAll(exercisesPagedTask, programsPagedTask, muscleDistributionTask, personalRecordsTask);

        var exercises = exercisesPagedTask.Result.Items.Where(e => e.IsActive && !e.IsDeleted).ToList();
        var programs = programsPagedTask.Result.Items.Where(p => !p.IsDeleted).ToList();

        // Validate program/split selection (drop invalid values silently).
        var selectedProgram = programId.HasValue ? programs.FirstOrDefault(p => p.Id == programId.Value) : null;
        var effectiveProgramId = selectedProgram?.Id;

        Guid? effectiveSplitId = null;
        if (selectedProgram is not null && splitId.HasValue)
        {
            var match = selectedProgram.Splits.FirstOrDefault(s => s.Id == splitId.Value && !s.IsDeleted);
            if (match is not null)
                effectiveSplitId = match.Id;
        }

        var volumeTrendTask = dashboardService.GetVolumeTrendAsync(days,
                                                                   period,
                                                                   effectiveProgramId,
                                                                   effectiveSplitId,
                                                                   cancellationToken);

        IReadOnlyList<ExerciseProgressPointDto> exerciseProgress = [];
        var selectedExerciseId = exerciseId ?? exercises.FirstOrDefault()?.Id;
        Task<IReadOnlyList<ExerciseProgressPointDto>>? exerciseProgressTask = null;
        if (selectedExerciseId is not null && selectedExerciseId != Guid.Empty)
        {
            exerciseProgressTask = dashboardService.GetExerciseProgressAsync(selectedExerciseId.Value,
                                                                             Math.Max(days, 90),
                                                                             cancellationToken);
        }

        await volumeTrendTask;
        if (exerciseProgressTask is not null)
            exerciseProgress = await exerciseProgressTask;

        var model = new AnalyticsViewModel
        {
            Days = days,
            Period = period,
            ExerciseId = selectedExerciseId,
            ProgramId = effectiveProgramId,
            SplitId = effectiveSplitId,
            Exercises = exercises,
            Programs = programs,
            VolumeTrend = volumeTrendTask.Result,
            MuscleGroupDistribution = muscleDistributionTask.Result,
            ExerciseProgress = exerciseProgress,
            PersonalRecords = personalRecordsTask.Result
        };

        return View(model);
    }
}
