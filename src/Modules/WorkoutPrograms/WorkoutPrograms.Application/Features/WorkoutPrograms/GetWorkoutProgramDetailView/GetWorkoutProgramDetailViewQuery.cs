using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramDetailView;

public sealed record GetWorkoutProgramDetailViewQuery(Guid Id) : IQuery<Result<WorkoutProgramDetailViewDto>>;
