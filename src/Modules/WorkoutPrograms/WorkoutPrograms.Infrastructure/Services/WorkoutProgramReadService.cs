using WorkoutPrograms.Application.Services;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Infrastructure.Services
{
    public class WorkoutProgramReadService(IWorkoutProgramRepository _workoutProgramRepository) : IWorkoutProgramReadService
    {
        public async Task<WorkoutProgram> GetWorkoutProgramByIdAsync(Guid workoutProgramId, CancellationToken cancellationToken = default)
        {
            return await _workoutProgramRepository.GetByIdAsync(workoutProgramId, cancellationToken);
        }

        public async Task<bool> ContainsExerciseAsync(Guid workoutProgramId, Guid exerciseId, CancellationToken cancellationToken = default)
        {
            var result = await _workoutProgramRepository.GetByIdAsync(workoutProgramId, cancellationToken);

            if (result is null)
            {
                return false;
            }

            return result.ContainsExercise(exerciseId);
        }

        public async Task<bool> ExistsAsync(Guid workoutProgramId, CancellationToken cancellationToken = default)
        {
            var result = await _workoutProgramRepository.GetByIdAsync(workoutProgramId, cancellationToken);

            if (result is null)
            {
                return false;
            }

            return true;
        }
    }
}
