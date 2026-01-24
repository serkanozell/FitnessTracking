using FitnessTracking.Domain.Repositories;
using MediatR;

namespace FitnessTracking.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    public sealed class DeleteWorkoutProgramCommandHandler
    : IRequestHandler<DeleteWorkoutProgramCommand, Unit>
    {
        private readonly IWorkoutProgramRepository _repository;

        public DeleteWorkoutProgramCommandHandler(IWorkoutProgramRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            DeleteWorkoutProgramCommand request,
            CancellationToken cancellationToken)
        {
            var program = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (program is null)
            {
                return Unit.Value; // Idempotent behavior
            }

            await _repository.DeleteAsync(request.Id, cancellationToken);

            return Unit.Value;
        }
    }
}