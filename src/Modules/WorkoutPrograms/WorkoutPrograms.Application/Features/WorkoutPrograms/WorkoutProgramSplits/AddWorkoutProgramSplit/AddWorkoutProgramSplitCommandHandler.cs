using WorkoutPrograms.Domain.Repositories;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    internal sealed class AddWorkoutProgramSplitCommandHandler(IWorkoutProgramRepository _workoutProgramRepository, IWorkoutProgramsUnitOfWork _unitOfWork) : ICommandHandler<AddWorkoutProgramSplitCommand, Guid>
    {
        public async Task<Guid> Handle(AddWorkoutProgramSplitCommand request, CancellationToken cancellationToken)
        {
            // Aggregate root'u, split ve exercises ile birlikte yükle
            var workoutProgram = await _workoutProgramRepository.GetByIdAsync(request.WorkoutProgramId, cancellationToken) ?? throw new KeyNotFoundException($"WorkoutProgram ({request.WorkoutProgramId}) not found.");

            // Davranışı aggregate üzerinden yap
            var split = workoutProgram.AddSplit(request.Name, request.Order);

            await _workoutProgramRepository.UpdateAsync(workoutProgram, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return split.Id;
        }
    }
}