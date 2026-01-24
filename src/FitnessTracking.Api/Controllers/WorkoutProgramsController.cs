using FitnessTracking.Application.Features.WorkoutPrograms.DeleteWorkoutProgram;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class WorkoutProgramsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkoutProgramsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Request models (API layer)
        public sealed class CreateWorkoutProgramRequest
        {
            public string Name { get; init; } = default!;
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
        }

        public sealed class UpdateWorkoutProgramRequest
        {
            public string Name { get; init; } = default!;
            public DateTime StartDate { get; init; }
            public DateTime EndDate { get; init; }
        }


        // GET: api/workoutprograms
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            // TODO: replace with real MediatR query when you add it:
            var result = await _mediator.Send(new GetWorkoutProgramListQuery(), cancellationToken);
            return Ok(result);
        }

        // GET: api/workoutprograms/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            // TODO: replace with real MediatR query when you add it:
            var result = await _mediator.Send(new GetWorkoutProgramByIdQuery { Id = id }, cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // POST: api/workoutprograms
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateWorkoutProgramRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // TODO: replace with real MediatR command when you add it:
            var id = await _mediator.Send(
                new CreateWorkoutProgramCommand { Name = request.Name, StartDate = request.StartDate, EndDate = request.EndDate },
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT: api/workoutprograms/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateWorkoutProgramRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // TODO: replace with real MediatR command when you add it:
            var success = await _mediator.Send(
                new UpdateWorkoutProgramCommand { Id = id, Name = request.Name, StartDate = request.StartDate, EndDate = request.EndDate },
                cancellationToken);

            return NoContent();

        }

        // DELETE: api/workoutprograms/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            // TODO: replace with real MediatR command when you add it:
            var success = await _mediator.Send(new DeleteWorkoutProgramCommand { Id = id }, cancellationToken);
            return NoContent();
        }



        // ---------- Program Exercises (child entity of WorkoutProgram) ----------

        public sealed class AddProgramExerciseRequest
        {
            public Guid ExerciseId { get; init; }
            public int Sets { get; init; }
            public int TargetReps { get; init; }
        }

        public sealed class UpdateProgramExerciseRequest
        {
            public int Sets { get; init; }
            public int TargetReps { get; init; }
        }

        // POST: api/workoutprograms/{programId}/exercises
        [HttpPost("{programId:guid}/exercises")]
        public async Task<IActionResult> AddExercise(
            Guid programId,
            [FromBody] AddProgramExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var workoutProgramExerciseId = await _mediator.Send(
                new AddExerciseToWorkoutProgramCommand
                {
                    WorkoutProgramId = programId,
                    ExerciseId = request.ExerciseId,
                    Sets = request.Sets,
                    TargetReps = request.TargetReps
                },
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = programId },
                workoutProgramExerciseId);
        }

        // PUT: api/workoutprograms/{programId}/exercises/{workoutProgramExerciseId}
        [HttpPut("{programId:guid}/exercises/{workoutProgramExerciseId:guid}")]
        public async Task<IActionResult> UpdateExercise(
            Guid programId,
            Guid workoutProgramExerciseId,
            [FromBody] UpdateProgramExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var success = await _mediator.Send(
                new UpdateWorkoutProgramExerciseCommand
                {
                    WorkoutProgramId = programId,
                    WorkoutProgramExerciseId = workoutProgramExerciseId,
                    Sets = request.Sets,
                    TargetReps = request.TargetReps
                },
                cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/workoutprograms/{programId}/exercises/{workoutProgramExerciseId}
        [HttpDelete("{programId:guid}/exercises/{workoutProgramExerciseId:guid}")]
        public async Task<IActionResult> RemoveExercise(
            Guid programId,
            Guid workoutProgramExerciseId,
            CancellationToken cancellationToken)
        {
            var success = await _mediator.Send(
                new RemoveWorkoutProgramExerciseCommand
                {
                    WorkoutProgramId = programId,
                    WorkoutProgramExerciseId = workoutProgramExerciseId
                },
                cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}