using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ExercisesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExercisesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/exercises
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ExerciseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllExercisesQuery(), cancellationToken);
            return Ok(result);
        }

        // GET: api/exercises/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetExerciseByIdQuery(id), cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        public sealed class CreateExerciseRequest
        {
            public string Name { get; init; } = default!;
            public string MuscleGroup { get; init; } = default!;
            public string Description { get; init; } = default!;
        }

        // POST: api/exercises
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var command = new CreateExerciseCommand(
                request.Name,
                request.MuscleGroup,
                request.Description);

            var id = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id },
                id);
        }

        public sealed class UpdateExerciseRequest
        {
            public string Name { get; init; } = default!;
            public string MuscleGroup { get; init; } = default!;
            public string Description { get; init; } = default!;
        }

        // PUT: api/exercises/{id}
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateExerciseRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var command = new UpdateExerciseCommand(
                id,
                request.Name,
                request.MuscleGroup,
                request.Description);

            var success = await _mediator.Send(command, cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/exercises/{id}
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteExerciseCommand(id);
            var success = await _mediator.Send(command, cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}