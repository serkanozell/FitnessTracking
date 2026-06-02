using BuildingBlocks.Domain.Abstractions;
using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Infrastructure.Specifications
{
    public sealed class WorkoutSessionsByUserSpecification : Specification<WorkoutSession>
    {
        public WorkoutSessionsByUserSpecification(Guid userId)
            : base(x => x.UserId == userId)
        {
            AddInclude(x => x.SessionExercises);
            ApplyOrderByDescending(x => x.Date);
            ApplyNoTracking();
        }
    }

    public sealed class WorkoutSessionsByUserAndProgramSpecification : Specification<WorkoutSession>
    {
        public WorkoutSessionsByUserAndProgramSpecification(Guid userId, Guid workoutProgramId)
            : base(x => x.UserId == userId && x.WorkoutProgramId == workoutProgramId)
        {
            AddInclude(x => x.SessionExercises);
            ApplyOrderByDescending(x => x.Date);
            ApplyNoTracking();
        }
    }

    public sealed class WorkoutSessionsByProgramSpecification : Specification<WorkoutSession>
    {
        public WorkoutSessionsByProgramSpecification(Guid workoutProgramId)
            : base(x => x.WorkoutProgramId == workoutProgramId)
        {
            AddInclude(x => x.SessionExercises);
            ApplyOrderByDescending(x => x.Date);
            ApplyNoTracking();
        }
    }

    public sealed class WorkoutSessionsPagedSpecification : Specification<WorkoutSession>
    {
        public WorkoutSessionsPagedSpecification()
        {
            AddInclude(x => x.SessionExercises);
            ApplyOrderByDescending(x => x.Date);
            ApplyNoTracking();
        }
    }
}
